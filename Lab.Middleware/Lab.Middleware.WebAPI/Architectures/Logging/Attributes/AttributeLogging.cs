using System;

namespace Lab.Middleware.WebAPI.Architectures.Logging.Attributes
{
    [LoggingFlags]
    [Flags]
    public enum AttributeLogging
    {
        None = 0,
        Error = 1,
        Request = 2,
        All = Error | Request
    }
}