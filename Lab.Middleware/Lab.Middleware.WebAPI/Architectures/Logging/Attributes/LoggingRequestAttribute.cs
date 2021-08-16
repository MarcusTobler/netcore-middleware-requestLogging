using System;

namespace Lab.Middleware.WebAPI.Architectures.Logging.Attributes
{
    public class LoggingRequestAttribute : Attribute, ILoggingTemplateProvider
    {
        private AttributeLogging _attributeLogging;
        
        public AttributeLogging AttributeLogging
        {
            get { return _attributeLogging; }
            set { _attributeLogging = value; }
        }
        
        public LoggingRequestAttribute()
        {
            _attributeLogging = AttributeLogging.Error;
        }

        public LoggingRequestAttribute(AttributeLogging attributeLogging)
        {
            _attributeLogging = attributeLogging;
        }
        
    }
}