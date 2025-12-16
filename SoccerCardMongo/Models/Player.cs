using MongoDB.Bson;

namespace SoccerCardMongo.Models
{
    public class Player
    {
        public ObjectId Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Position { get; set; }
        public List<Card> Cards { get; set; } = new();
    }
}