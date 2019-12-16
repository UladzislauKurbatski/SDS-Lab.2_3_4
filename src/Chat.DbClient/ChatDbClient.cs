using Chat.Domain.Entities;
using Chat.Domain.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Chat.DbClient
{
    public class ChatDbClient : IChatClient<Message, IEnumerable<Message>>
    {
        public const string MEDIA_TYPE = "application/json";

        public IEnumerable<Message> Send(Message message)
        {
            var serializedMessage = JsonConvert.SerializeObject(message);
            using (var httpClient = new HttpClient())
            using (var content = new  StringContent(serializedMessage, Encoding.UTF8, MEDIA_TYPE))
            using (var response = httpClient.PostAsync("http://localhost:5000/api/message/database", content).GetAwaiter().GetResult())
            {
                var responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return JsonConvert.DeserializeObject<IEnumerable<Message>>(responseString);
            }
        }
    }
}
