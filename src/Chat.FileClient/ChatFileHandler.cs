using Chat.Domain.Entities;
using Chat.Domain.Interfaces;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Chat.FileClient
{
    public class ChatFileHandler : IChatFilesHandler
    {
        private readonly IChatClient<byte[], byte[]> _chatClient;
        private readonly string _projectPath;

        public ChatFileHandler(IChatClient<byte[], byte[]> chatClient, string projectPath)
        {
            _chatClient = chatClient;
            _projectPath = projectPath;
        }

        public void Handle(string username)
        {
            Console.Write("Type your message: ");
            var userInput = Console.ReadLine();
            while (userInput != "/exit")
            {
                ClearCurrentConsoleLine();
                byte[] fileBuffer = WriteFile(username, userInput);
                var response = _chatClient.Send(fileBuffer);
                Console.WriteLine(ParseResponse(response));
                Console.Write("Type your message: ");
                userInput = Console.ReadLine();
            }
        }

        private byte[] WriteFile(string userName, string text)
        {
            using (var ms = new MemoryStream())
            using (var package = new ExcelPackage(ms))
            {
                var worksheets = package.Workbook.Worksheets;
                var worksheet = worksheets.Add("Message");

                worksheet.Cells[1, 1].Value = userName;
                worksheet.Cells[1, 2].Value = DateTime.UtcNow.Ticks.ToString();
                worksheet.Cells[1, 3].Value = text;

                package.Save();
                ms.Position = 0;
                var buffer = new byte[ms.Length];
                ms.Read(buffer, 0, (int)ms.Length);
                return buffer;
            }
        }

        private static byte[] ReadFile(string filePath)
        {
            byte[] buffer;
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            try
            {
                int length = (int)fileStream.Length;  // get file length
                buffer = new byte[length];            // create buffer
                int count;                            // actual number of bytes read
                int sum = 0;                          // total number of bytes read

                // read until Read method returns 0 (end of the stream has been reached)
                while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
                    sum += count;  // sum is a buffer offset for next reading
            }
            finally
            {
                fileStream.Close();
            }
            return buffer;
        }

        private string ParseResponse(byte[] file)
        {
            var messages = new List<Message>();
            using(var ms = new MemoryStream(file))
            using (var package = new ExcelPackage(ms))
            {
                var worksheet = package.Workbook.Worksheets.First();
                var row = 1;

                while (worksheet.Cells[row, 1].Value != null)
                {
                    var message = new Message
                    {
                        UserName = worksheet.Cells[row, 1].Value as string,
                        Date = new DateTime(Int64.Parse(worksheet.Cells[row, 2].Value as string)),
                        Text = worksheet.Cells[row, 3].Value as string,
                    };
                    row++;
                    messages.Add(message);
                }
                return string.Join("\n", messages.Select(x => x.ToString()));
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
