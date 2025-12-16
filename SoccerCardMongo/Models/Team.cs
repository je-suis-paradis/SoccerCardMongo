using MongoDB.Bson;

namespace SoccerCardMongo.Models
{
    public class Team
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public List<Player> Players { get; set; } = new();
    }
}