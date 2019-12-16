using System.IO;
using System.Linq;
using Chat.Domain.Interfaces;
using OfficeOpenXml;
using System;

namespace Chat.FileServer
{
    public class ChatFileServerService : IChatServerService<byte[], byte[]>
    {
        private readonly string _outputFilePath;
        private readonly string _outputDirectoryName;
        private readonly string _projectDirectoryPath;

        public ChatFileServerService(ChatFileServerServiceConfiguration configuration, string projectDirectoryPath)
        {
            _outputFilePath = configuration.OutputFileName;
            _outputDirectoryName = configuration.OutputDirectoryName;
            _projectDirectoryPath = projectDirectoryPath;
        }

        public byte[] Process(byte[] message)
        {
            GetData(message, out var userName, out var userMessage, out DateTime date);

            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException($"Username can't be empty");
            }

            if (string.IsNullOrWhiteSpace(userMessage))
            {
                throw new ArgumentException($"User message can't be empty");
            }

            var filePath = Path.Combine(_projectDirectoryPath, _outputDirectoryName, _outputFilePath);
            if (!File.Exists(filePath))
            {
                File.Create(filePath);
            }

            byte[] responseBuffer;

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheets = package.Workbook.Worksheets;
                var worksheet = worksheets.FirstOrDefault() ?? worksheets.Add("ChatHistory");

                responseBuffer = GetResponse(worksheet, userName, date, userMessage);

                worksheet.Column(1).Width = 100;
                worksheet.Column(2).Width = 50;
                worksheet.Column(3).Width = 100;

                var lastRow = (worksheet.Dimension?.End.Row ?? 0) + 1; 
                
                worksheet.Cells[lastRow, 1].Value = userName;
                worksheet.Cells[lastRow, 2].Value = date.Ticks.ToString();
                worksheet.Cells[lastRow, 3].Value = userMessage;
                package.Save();
            }

            return responseBuffer;
        }

        private byte[] GetResponse(ExcelWorksheet worksheet, string userName, DateTime date, string userMessage)
        {
            using (var ms = new MemoryStream())
            using (var package = new ExcelPackage(ms))
            {
                var lastRow = worksheet.Dimension?.End.Row ?? 0;

                while (lastRow > 1 && !string.IsNullOrEmpty(worksheet.Cells[lastRow, 1].Value as string) && (worksheet.Cells[lastRow, 1].Value as string) != userName) lastRow--;

                lastRow++;

                var ws = package.Workbook.Worksheets.Add("ChatHistory");
                var currentRow = 1;
                while (lastRow <= (worksheet.Dimension?.End.Row ?? 0))
                {
                    ws.Cells[currentRow, 1].Value = worksheet.Cells[lastRow, 1].Value;
                    ws.Cells[currentRow, 2].Value = worksheet.Cells[lastRow, 2].Value;
                    ws.Cells[currentRow, 3].Value = worksheet.Cells[lastRow, 3].Value;
                    currentRow++;
                    lastRow++;
                }

                ws.Cells[currentRow, 1].Value = userName;
                ws.Cells[currentRow, 2].Value = date.Ticks.ToString();
                ws.Cells[currentRow, 3].Value = userMessage;

                package.Save();
                ms.Position = 0;
                var buffer = new byte[ms.Length];
                ms.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }

        private void GetData(byte[] buffer, out string userName, out string userMessage, out DateTime date)
        {
            using (var ms = new MemoryStream(buffer))
            using (var package = new ExcelPackage(ms))
            {
                var worksheet = package.Workbook.Worksheets.First();

                userName = worksheet.Cells[1, 1].Value as string;
                date = new DateTime(Int64.Parse(worksheet.Cells[1, 2].Value as string));
                userMessage = worksheet.Cells[1, 3].Value as string;
            }
        }
    }
}
