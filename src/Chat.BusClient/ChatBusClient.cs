using Chat.BusClient.Contracts;
using Chat.Domain.Interfaces;
using MassTransit;
using RabbitMQ.Client;
using System;

namespace Chat.BusClient
{
    public interface IChatBusClient : IChatClient<IChatMessage, bool>
    {
    }

    public class ChatBusClient : IChatBusClient, IDisposable
    {
        private readonly IBusControl _busControl; 


        public ChatBusClient(string username)
        {
            _busControl = CreateBus(username);
            _busControl.Start();
        }

        public bool Send(IChatMessage message)
        {
            try
            {
                _busControl.Publish<IChatMessage>(message)
                .ConfigureAwait(false)
                .GetAwaiter().GetResult();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private IBusControl CreateBus(string username)
        {

            return Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(new Uri("rabbitmq://localhost"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
                cfg.ExchangeType = ExchangeType.Fanout;
            });
        }

        public void Dispose()
        {
            _busControl.Stop();
        }
    }
}
