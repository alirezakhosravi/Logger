using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Raveshmand.Logger
{
    [ProviderAlias("EntityFramework")]
    public class LoggerProvider<TContext> : ILoggerProvider
        where TContext : DbContext
    {
        private readonly Func<string, LogLevel, bool> _filter;
        private readonly IServiceProvider _serviceProvider;

        public LoggerProvider(Func<string, LogLevel, bool> filter,
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _filter = filter ?? throw new ArgumentNullException(nameof(filter));
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new Logger<TContext>(this._serviceProvider, categoryName, this._filter);
        }

        public void Dispose()
        {
        }
    }
}
