using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Lab.Middleware.WebAPI.Architectures.Logging
{
    public static class RequestResponseLoggingMiddlewareRegister
    {
        public static void UseRequestResponseLogging(this IApplicationBuilder app)
        {
            app.UseMiddleware<RequestResponseLoggingMiddleware>();
        }

        public static void AddRequestResponseLogging(this IServiceCollection services)
        {
            
        }
    }
}