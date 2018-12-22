using System;
using Microsoft.Extensions.Logging;

namespace Raveshmand.Logger
{
    public class LoggerOption
    {
        public string LoggerName { get; set; }
        public Func<string, LogLevel, bool> Filter { get; set; }
    }
}
