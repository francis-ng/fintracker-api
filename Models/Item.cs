using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FinancialTrackerApi.Models
{
    public class Item
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Label { get; set; }

        public decimal Amount { get; set; }
    }
}
