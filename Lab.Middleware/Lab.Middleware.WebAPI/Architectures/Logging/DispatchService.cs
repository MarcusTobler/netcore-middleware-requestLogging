using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Lab.Middleware.WebAPI.Architectures.Logging
{
    public class DispatchService : IDisposable
    {
        public void Dispatch(LogEntry log, string queueToSend)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                VirtualHost = "EnterpriseLog",
                UserName = "admin",
                Password = "P@ssw0rd"
            };
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                string message = JsonConvert.SerializeObject(log);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                    routingKey: queueToSend,
                    basicProperties: null,
                    body: body);
            }
        }
        
        public void Dispose()
        {
        }
    }
}