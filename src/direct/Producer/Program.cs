using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Linq;
using System.Text;

namespace Direct.Producer
{
    class Program
    {
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost", VirtualHost = "development" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "emails.direct",
                                        type: "direct", durable: true);

                var name = (args.Length > 0) ? args[0] : "siaidp";
                var message = (args.Length > 1)
                              ? string.Join(" ", args.Skip(1).ToArray())
                              : "Hello World!";
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "emails.direct",
                                     routingKey: name,
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine(" [x] Enviado '{0}':'{1}'", name, message);
            }

            Console.WriteLine(" Tecle [enter] para sair.");
            Console.ReadLine();
        }
    }
}
