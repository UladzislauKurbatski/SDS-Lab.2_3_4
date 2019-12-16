using System;
using Chat.Domain.Interfaces;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Chat.FileClient
{
    public class ChatFileClient : IChatClient<byte[], byte[]>
    {
        public const string MEDIA_TYPE = "application/vnd.ms-excel";

        public byte[] Send(byte[] message)
        {
            using (var httpClient = new HttpClient())
            using (var messageContent = new ByteArrayContent(message))
            using (var content = new MultipartFormDataContent())
            {
                messageContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MEDIA_TYPE);

                var fileName = $"{DateTime.UtcNow.ToString("yyyy-MM-dd")}-{Guid.NewGuid()}";
                content.Add(messageContent, fileName, fileName);
               
                using (var response = httpClient.PostAsync("http://localhost:5000/api/message/file", content).GetAwaiter().GetResult())
                using(var responseStream = response.Content.ReadAsStreamAsync().GetAwaiter().GetResult())
                {
                    var buffer = new byte[responseStream.Length];
                    responseStream.Read(buffer, 0, buffer.Length);
                    return buffer;
                }
            }
        }
    }
}
