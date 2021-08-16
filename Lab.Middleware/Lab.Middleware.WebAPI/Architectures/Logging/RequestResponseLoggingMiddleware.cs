using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Lab.Middleware.WebAPI.Architectures.Logging.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace Lab.Middleware.WebAPI.Architectures.Logging
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestResponseLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var log = new LogEntry()
            {
                ProjectKey = "Lab.Middleware",
                Date = DateTime.Now
            };
            
            log.Tags = new Dictionary<string, object>()
            {
                { "Path", context.Request.Path },
                { "Scheme", context.Request.Scheme },
                { "Method", context.Request.Method },
                { "Host", context.Request.Host.ToString() },
                { "RemoteIdAddress", context.Connection.RemoteIpAddress?.ToString() },
                { "QueryString", context.Request.QueryString.Value }
            };

            FormatRequest(log, context.Request);
            
            context.Items.Add("logentry", log);
            
            try
            {
                await _next(context);
                log.LogLevel = LogLevel.Request;
            }
            catch (Exception ex)
            {
                log.LogLevel = LogLevel.Error;
                //AddOrUpdateTag(log, "Exception", ex.ToString());
                AddOrUpdateTag(log, "Exception.Message", ex.Message);
                AddOrUpdateTag(log, "Exception.InnerException", ex.InnerException?.Message ?? "");
                throw;
            }
            finally
            {
                context.Items.Remove("logentry");
                PostExecutionLog(context, log);

                var loggingRequestAttribute = context.Features.Get<IEndpointFeature>()?.Endpoint?.Metadata.GetMetadata<LoggingRequestAttribute>();
                
                if (log.LogLevel == LogLevel.Error)
                {
                    if (loggingRequestAttribute != null && loggingRequestAttribute.AttributeLogging != AttributeLogging.None)
                    {
                        var dispatchService = new DispatchService();
                        dispatchService.Dispatch(log, "ApplicationLog");
                    }
                }
                else
                {
                    if (loggingRequestAttribute != null && 
                        (loggingRequestAttribute.AttributeLogging == AttributeLogging.All ||
                         loggingRequestAttribute.AttributeLogging == AttributeLogging.Request))
                    {
                        var dispatchService = new DispatchService();
                        dispatchService.Dispatch(log, "ApplicationLog");
                    }
                }


                

            }
            
        }

        private void FormatRequest(LogEntry log, HttpRequest request)
        {
            var accessToken = request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            if (!string.IsNullOrEmpty(accessToken))
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(accessToken);
                var groupAd = token.Claims.First(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
                var uniqueName = token.Claims.First(c => c.Type == "unique_name")?.Value;
                
                AddOrUpdateTag(log, "User.Token", token);
                AddOrUpdateTag(log, "User.GroupAD", groupAd);
                AddOrUpdateTag(log, "User.UniqueName", uniqueName);
            }
                
            if (request.Query.Any())
                request.Query.ToList().ForEach(x =>
                {
                    AddOrUpdateTag(log, $"_query._{x.Key}", x.Value);
                });
            
            AddOrUpdateTag(log, "Request.Lenght", request.ContentLength ?? 0);
        }
        
        private void AddOrUpdateTag(LogEntry log, string key, object value)
        {
            if (key != null && key.Contains("__") == false)
            {
                if (log.Tags.ContainsKey(key))
                    log.Tags[key] = value.ToString();
                else
                    log.Tags.Add(key, value.ToString());
            }
        }

        private void PostExecutionLog(HttpContext context, LogEntry log)
        {
            if (context.Items != null && context.Items.Any())
            {
                foreach (var item in context.Items)
                {
                    if (item.Key is string)
                        AddOrUpdateTag(log, (string)item.Key, item.Value);
                }
            }

            AddOrUpdateTag(log, "Response.Length", context.Response?.ContentLength ?? 0);
            AddOrUpdateTag(log, "Response.StatusCode", context.Response?.StatusCode);
            
        }

        private void SendLoggingRequest(LogEntry log)
        {
            var jsonString = JsonConvert.SerializeObject(log);
                
            Console.WriteLine("----------------------------------");
            Console.WriteLine(jsonString);
        }
    }
}