using NLog.LayoutRenderers;
using RabbitMQ.Client;
using System.Text;
using System;
using System.Threading;

namespace LoginRegisterAPI.RabbitMQService
{
    public class MessagePublish
    {
        public void sendMessage(string message)
        {
            //publish message
            //connection to server
            var connectionFac = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };
            var connection = connectionFac.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "letterbox",
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            //create message that will be publish
            
            
            var encMessage = Encoding.UTF8.GetBytes(message);
            //publish message
            channel.BasicPublish("", "letterbox", null, encMessage);

            Console.WriteLine($"published message: {message}");

            connection.Close();
        }
    }
}
