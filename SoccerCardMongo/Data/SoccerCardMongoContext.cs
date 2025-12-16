using MongoDB.Driver;
using SoccerCardMongo.Models;

namespace SoccerCardMongo.Data
{
    public class SoccerCardMongoContext
    {
        private readonly IMongoDatabase _database;
        public IMongoCollection<Team> Teams { get; }

        public SoccerCardMongoContext(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
            Teams = _database.GetCollection<Team>("team");
        }
    }
}