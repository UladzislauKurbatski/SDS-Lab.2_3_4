using Chat.Domain.Entities;
using Chat.Domain.Interfaces;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace Chat.DbServer
{
    public class ChatDbServerService : IChatServerService<Message, IEnumerable<Message>>
    {
        public const string CONNECTION_STRING_NAME = "MongoDb";

        private readonly IMongoCollection<Message> _collection;
        private readonly IMongoDatabase _database;

        static ChatDbServerService()
        {
            BsonClassMap.RegisterClassMap<Message>(x => {
                x.AutoMap();
                x.SetIgnoreExtraElements(true);
            });
        }

        public ChatDbServerService(string connectionString)
        {
            var mongodbUrl = new MongoUrl(connectionString);
            var client = new MongoClient(mongodbUrl);
            _database = client.GetDatabase(mongodbUrl.DatabaseName);
            _collection = _database.GetCollection<Message>(typeof(Message).Name);
        }

        public IEnumerable<Message> Process(Message message)
        {
            var messages = _collection.Find(Builders<Message>.Filter.Empty).ToList().OrderBy(x => x.Date).ToList();
            var lastUserMessage = messages.LastOrDefault(x => x.UserName == message.UserName);

            _collection.InsertOne(message);
            messages.Add(message);

            return lastUserMessage == null ? messages : messages.Where(x => x.Date > lastUserMessage.Date);
        }
    }
}
