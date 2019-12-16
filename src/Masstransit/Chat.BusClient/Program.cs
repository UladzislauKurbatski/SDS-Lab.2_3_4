using MassTransit;
using MassTransit.RabbitMqTransport;
using RabbitMQ.Client;
using System;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Chat.BusClient
{
    

    class Program
    {
        static void Main(string[] args)
        {
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
                cfg.ReceiveEndpoint("event_queue", e =>
                {
                    e.Handler<ValueEntered>(context =>
                        Console.Out.WriteLineAsync($"Value was entered: {context.Message.Value}"));
                });
            });

            // Important! The bus must be started before using it!
            await busControl.StartAsync();
            try
            {
                do
                {
                    string value = await Task.Run(() =>
                    {
                        Console.WriteLine("Enter message (or quit to exit)");
                        Console.Write("> ");
                        return Console.ReadLine();
                    });

                    if ("quit".Equals(value, StringComparison.OrdinalIgnoreCase))
                        break;

                    await busControl.Publish<ValueEntered>(new
                    {
                        Value = value
                    });
                }
                while (true);
            }
            finally
            {
                await busControl.StopAsync();
            }
        }
    }

    public class HostSettings : RabbitMqHostSettings
    {
        public string Host {get;set;}

        public int Port {get;set;}

        public string VirtualHost {get;set;}

        public string Username {get;set;}

        public string Password {get;set;}

        public ushort Heartbeat {get;set;}

        public bool Ssl {get;set;}

        public SslProtocols SslProtocol {get;set;}

        public string SslServerName {get;set;}

        public SslPolicyErrors AcceptablePolicyErrors {get;set;}

        public string ClientCertificatePath {get;set;}

        public string ClientCertificatePassphrase {get;set;}

        public X509Certificate ClientCertificate {get;set;}

        public bool UseClientCertificateAsAuthenticationIdentity {get;set;}

        public LocalCertificateSelectionCallback CertificateSelectionCallback {get;set;}
        public RemoteCertificateValidationCallback CertificateValidationCallback {get;set;}

        public string[] ClusterMembers {get;set;}

        public IRabbitMqEndpointResolver HostNameSelector {get;set;}

        public string ClientProvidedName {get;set;}

        public Uri HostAddress {get;set;}

        public bool PublisherConfirmation {get;set;}

        public ushort RequestedChannelMax {get;set;}
    }

}
