using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace Producer
{
    class Program
    {
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost", VirtualHost="development" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "emails.topic",
                                        type: "topic", durable: true);
                var routingKey = (args.Length > 0) ? args[0] : "emails.anonymous";
                var message = (args.Length > 1)
                              ? string.Join(" ", args.Skip(1).ToArray())
                              : "Hello World!";
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "emails.topic",
                                     routingKey: routingKey,
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine(" [x] Routing key '{0}'. Message:'{1}'", routingKey, message);
            }
        }
    }
}
