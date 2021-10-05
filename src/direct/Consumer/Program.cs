using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Direct.Consumer
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
                var queueName = channel.QueueDeclare().QueueName;

                if (args.Length < 1)
                {
                    Console.Error.WriteLine("Uso: {0} [email_queue_name]",
                                            Environment.GetCommandLineArgs()[0]);
                    Console.WriteLine(" Tecle [enter] para sair.");
                    Console.ReadLine();
                    Environment.ExitCode = 1;
                    return;
                }

                foreach (var name in args)
                {
                    channel.QueueBind(queue: queueName,
                                      exchange: "emails.direct",
                                      routingKey: name);
                }

                Console.WriteLine(" [*] Aguardando mensagens.");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var routingKey = ea.RoutingKey;
                    Console.WriteLine(" [x] Recebeu '{0}':'{1}'",
                                      routingKey, message);
                };
                channel.BasicConsume(queue: queueName,
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine(" Tecle [enter] para sair.");
                Console.ReadLine();
            }
        }
    }
}
