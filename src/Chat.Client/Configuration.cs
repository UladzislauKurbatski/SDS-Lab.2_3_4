using Microsoft.Extensions.DependencyInjection;
using Chat.Domain.Interfaces;
using Chat.FileClient;
using System;
using System.IO;
using Chat.Domain.Entities;
using Chat.DbClient;
using System.Collections.Generic;
using Chat.BusClient;

namespace Chat.Client
{
    static class Configuration
    {
        private static string ProjectPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

        public static IServiceProvider Configure(IServiceCollection serviceCollection)
        {
            return serviceCollection
            .AddScoped<IChatClient<byte[], byte[]>, ChatFileClient>()
            .AddScoped<IChatClient<Message, IEnumerable<Message>>, ChatDbClient>()
            .AddScoped<IChatFilesHandler, ChatFileHandler>(provider =>
            {
                return new ChatFileHandler(provider.GetService<IChatClient<byte[], byte[]>>(), ProjectPath);
            })
            .AddScoped<IChatDbHandler, ChatDbHandler>()
            .AddScoped<IChatBusHandler, ChatBusHandler>()
            .BuildServiceProvider();
        }
    }
}
