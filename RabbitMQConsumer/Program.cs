using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Threading;
using System.Text;

namespace RabbitMQConsumer
{
    internal class Program
    {
        static private readonly ManualResetEvent _manualResetEvent = new ManualResetEvent(false);
        static void Main(string[] args)
        {

            //part2
            //consume message
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

            var consumer = new EventingBasicConsumer(channel);

            //deleiver message from queue
            //it push message
            consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"Received : {message}");
                };

            channel.BasicConsume(queue: "letterbox",
                             autoAck: true,
                             consumer: consumer);
            _manualResetEvent.WaitOne();

            connection.Close();

        }
    }
    
    
}
