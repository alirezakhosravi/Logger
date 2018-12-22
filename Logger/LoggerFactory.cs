using Microsoft.Extensions.Logging;

namespace Raveshmand.Logger
{
    public class LoggerFactory : ILoggerFactory
    {
        private ILoggerProvider _loggerProvider;

        public LoggerFactory(ILoggerProvider loggerProvider)
        {
            this._loggerProvider = loggerProvider;
        }

        public void AddProvider(ILoggerProvider provider)
        {
            this._loggerProvider = provider;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return this._loggerProvider.CreateLogger(categoryName);
        }

        public void Dispose()
        {
        }
    }
}
