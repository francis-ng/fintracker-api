using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Linq;

namespace FinancialTrackerApi.Models
{
    public class MonthlyLedger
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Owner { get; set; }

        public string Type { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }

        public DateTime UpdatedAt { get; set; }

        public Item[] Debits { get; set; }

        public decimal DebitTotal => Debits == null ? 0 : Debits.Sum(debit => debit.Amount);

        public Item[] Credits { get; set; }

        public decimal CreditTotal => Credits == null ? 0 : Credits.Sum(credit => credit.Amount);
    }
}
