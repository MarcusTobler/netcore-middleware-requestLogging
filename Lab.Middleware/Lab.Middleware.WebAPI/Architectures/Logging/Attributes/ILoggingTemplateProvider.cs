namespace Lab.Middleware.WebAPI.Architectures.Logging.Attributes
{
    public interface ILoggingTemplateProvider
    {
        AttributeLogging AttributeLogging { get; }
    }
}