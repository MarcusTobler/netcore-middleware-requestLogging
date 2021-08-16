using System;
using System.Collections.Generic;

namespace Lab.Middleware.WebAPI.Architectures.Logging
{
    public class LogEntry
    {
        public string ProjectKey { get; set; }
        public DateTime Date { get; set; }
        public Dictionary<string, object> Tags { get; set; }
        public LogLevel LogLevel { get; set; }

        public string LogLevelName => LogLevel.ToString();
    }
}