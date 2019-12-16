using Chat.Domain.Entities;
using Chat.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chat.DbClient
{
    public class ChatDbHandler : IChatDbHandler
    {
        private readonly IChatClient<Message, IEnumerable<Message>> _chatClient;

        public ChatDbHandler(IChatClient<Message, IEnumerable<Message>> chatClient)
        {
            _chatClient = chatClient;
        }

        public void Handle(string username)
        {
            Console.Write("Type your message: ");
            var userInput = Console.ReadLine();
            while(userInput != "/exit")
            {
                ClearCurrentConsoleLine();
                var response = _chatClient.Send(new Message { UserName = username, Text = userInput, Date = DateTime.UtcNow });
                var responseString = string.Join("\n", response.Select(x => x.ToString()));
                Console.WriteLine(responseString);
                Console.Write("Type your message: ");
                userInput = Console.ReadLine();
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
