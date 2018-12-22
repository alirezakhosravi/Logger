using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Raveshmand.Logger.Extentions
{
    public static class LoggerFactoryExtentions
    {
        public static ILoggerFactory AddEntityFramework<TContext>(this ILoggerFactory loggerFactory,
            IServiceProvider serviceProvider, LogLevel minLevel)
            where TContext : DbContext
        {
            bool LogFilter(string loggerName, LogLevel logLevel) => logLevel >= minLevel;

            loggerFactory.AddProvider(new LoggerProvider<TContext>(LogFilter, serviceProvider));

            return loggerFactory;
        }

        public static void AddLogger<TContext>(this IServiceCollection services, Action<LoggerOption> options)
            where TContext : DbContext
        {
            services.Configure(options);
            services.Add(ServiceDescriptor.Transient<ILogger, Logger<TContext>>());
        }

        public static void ApplyLogConfiguration(this ModelBuilder builder)
        {
            if(builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.ApplyConfiguration(new LogConfiguration());
        }
    }
}
