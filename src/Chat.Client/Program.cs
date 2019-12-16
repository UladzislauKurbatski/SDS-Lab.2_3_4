using System.IO;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Chat.Domain.Interfaces;
using System;
using System.Collections.Generic;
using Chat.FileClient;
using Chat.DbClient;
using Chat.BusClient;

namespace Chat.Client
{
    class Program
    {
        private static IServiceProvider serviceProvider = Configuration.Configure(new ServiceCollection());

        private readonly static IDictionary<string, IChatHandler> InteractionHandlers 
            = new Dictionary<string, IChatHandler>
        {
            { "file",  serviceProvider.GetService<IChatFilesHandler>()},
            { "database",  serviceProvider.GetService<IChatDbHandler>()},
            { "bus",  serviceProvider.GetService<IChatBusHandler>()},

        };

        static void Main(string[] args)
        {
            Console.Write("Type way of interaction: ");
            var interactionWay = Console.ReadLine();

            Console.Write("Type your login: ");
            var username = Console.ReadLine();

            if (InteractionHandlers.TryGetValue(interactionWay, out var handler))
            {
                handler.Handle(username);
            }
        }
    }
}
