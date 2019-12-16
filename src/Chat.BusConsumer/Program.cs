using Chat.BusClient.Contracts;
using MassTransit;
using RabbitMQ.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.BusConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (string.IsNullOrWhiteSpace(args[0]))
            {
                throw new ArgumentNullException("queueName");
            }

            Console.WriteLine(args[0]);

            Task.WaitAll(Task.Run(() => MainAsync(args)));
        }

        public static async Task MainAsync(string[] args)
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(new Uri("rabbitmq://localhost"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
                cfg.ExchangeType = ExchangeType.Fanout;
                cfg.ReceiveEndpoint(args[0], e =>
                {
                    e.Handler<IChatMessage>(context =>
                        Console.Out.WriteLineAsync($"\n<[{DateTime.UtcNow.ToString("dddd, dd MMMM yyyy HH:mm:ss")} {context.Message.User}]> {context.Message.Message}"));
                });
            });

            // Important! The bus must be started before using it!
            await busControl.StartAsync();

            Console.WriteLine("Bus started.");

            try
            {
                do
                {
                    Thread.Sleep(1000);
                }
                while (true);
            }
            finally
            {
                await busControl.StopAsync();
            }
        }
    }
}
