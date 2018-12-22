using System;

namespace Raveshmand.Logger 
{
    /// <summary>
    /// Represents a log in the logging database.
    /// </summary>
    public class LogRecord
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string Level { get; set; }
        public string Logger { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public string StateJson { get; set; }
        public string Url { get; set; }
        public DateTimeOffset CreationDateTime { get; set; }
    }
}
