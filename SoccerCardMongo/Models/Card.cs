using MongoDB.Bson;

namespace SoccerCardMongo.Models
{
    public class Card
    {
        public ObjectId Id { get; set; }
        public string CardNumber { get; set; }
        public string Rarity { get; set; }
        public int Power { get; set; }
    }
}