using Chat.BusClient.Contracts;
using System;
using System.Diagnostics;
using System.IO;

namespace Chat.BusClient
{
    public class ChatBusHandler : IChatBusHandler
    {
        private const string ConsumerPath = @"Chat.BusConsumer\bin\x64\Release\netcoreapp2.2\win10-x64\Chat.BusConsumer.exe";

        public void Handle(string username)
        {
            var solutionDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
            var consumerDirectory = Path.Combine(solutionDirectory, ConsumerPath);

            using (var chatBusClient = new ChatBusClient(username))
            using (var uiProcess = new Process())
            {
                uiProcess.StartInfo.FileName = consumerDirectory;
                uiProcess.StartInfo.Arguments = $"event_queue_{username}";
                uiProcess.StartInfo.CreateNoWindow = true;
                uiProcess.StartInfo.UseShellExecute = true;
                uiProcess.Start();

                do
                {
                    Console.Write("Type message:");
                    var message = Console.ReadLine();
                    if ("/exit".Equals(message, StringComparison.OrdinalIgnoreCase))
                        break;

                    ClearCurrentConsoleLine();
                    chatBusClient.Send(new ChatMessage
                    {
                        Message = message,
                        User = username,
                    });
                }
                while (true);
                uiProcess.CloseMainWindow();
            }
        }

        private void ClearCurrentConsoleLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }
    }
}
