using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace FinancialTrackerApi.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public byte[] Salt { get; set; }

        public string RefreshToken { get; set; }

        public string AccessToken { get; set; }

        public DateTime AccessExpiry { get; set; }
    }
}
