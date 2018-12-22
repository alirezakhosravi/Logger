using System;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Raveshmand.Logger.Extentions;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace Raveshmand.Logger
{
    public class Logger<TContext> : ILogger
        where TContext : DbContext
    {
        private readonly Func<string, LogLevel, bool> _filter;
        private readonly string _loggerName;
        private readonly IServiceProvider _serviceProvider;

        public Logger(IServiceProvider serviceProvider,
            IOptions<LoggerOption> options)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            if(options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if(string.IsNullOrEmpty(options.Value.LoggerName))
            {
                throw new ArgumentNullException(nameof(options.Value.LoggerName));
            }

            _loggerName = options.Value.LoggerName;
            _filter = options.Value.Filter ?? throw new ArgumentNullException(nameof(options.Value.Filter));
        }

        public Logger(IServiceProvider serviceProvider,
            string loggerName,
            Func<string, LogLevel, bool> filter)
        {
            if (string.IsNullOrEmpty(loggerName))
            {
                throw new ArgumentNullException(nameof(loggerName));
            }

            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _loggerName = loggerName;
            _filter = filter ?? throw new ArgumentNullException(nameof(filter));
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return NoopDisposable.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            // internal check to not log any Microsoft.EntityFrameworkCore.
            // It won't work any way and cause StackOverflowException
            if (_loggerName.StartsWith("Microsoft.EntityFrameworkCore", StringComparison.OrdinalIgnoreCase))
                return false;

            return _filter(_loggerName, logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            if (exception != null) message = $"{message}{Environment.NewLine}{exception}";

            if (string.IsNullOrEmpty(message)) return;

            WriteMessage(message, logLevel, eventId.Id, state, exception);
        }

        private void WriteMessage<TState>(string message, LogLevel logLevel, int eventId, TState state,
            Exception exception)
        {
            try
            {
                var httpContextAccessor = _serviceProvider.GetService<IHttpContextAccessor>();

                var log = new LogRecord
                {
                    EventId = eventId,
                    Level = logLevel.ToString(),
                    Logger = _loggerName,
                    Message = message,
                    Exception = exception?.Message,
                    Url = httpContextAccessor?.HttpContext?.Request?.Path.ToString(),
                    CreationDateTime = DateTimeOffset.UtcNow,
                };

                SetStateJson(state, log);

                // We need a separate context for the logger to call its SaveChanges several times,
                // without using the current request's context and changing its internal state.
                try
                {
                    _serviceProvider.RunScopedService<TContext>(context =>
                    {
                        context.Set<LogRecord>().Add(log);
                        context.SaveChanges();
                    });
                }
                catch(Exception ex)

                {
                    string path = $"{AppContext.BaseDirectory}/Log/";
                    string fileName = "Log.json";
                    List<LogRecord> logs = IO.JsonReader.Read<List<LogRecord>>(path, fileName);

                    log = new LogRecord
                    {
                        EventId = eventId,
                        Level = LogLevel.Critical.ToString(),
                        Logger = _loggerName,
                        Message = ex.Message,
                        Exception = ex?.InnerException?.Message,
                        Url = null,
                        CreationDateTime = DateTimeOffset.UtcNow,
                    };

                    logs.Add(log);

                    IO.JsonReader.Write(log, path, fileName);
                }
            }
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
            catch
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
            {
                // don't throw exceptions from logger
            }
        }

        private static void SetStateJson<TState>(TState state, LogRecord log)
        {
            log.StateJson = JsonConvert.SerializeObject(
                state,
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    DefaultValueHandling = DefaultValueHandling.Include
                });
        }

        private class NoopDisposable : IDisposable
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static NoopDisposable()
            {
            }

            public static NoopDisposable Instance { get; } = new NoopDisposable();

            public void Dispose()
            {
            }
        }
    }
}
