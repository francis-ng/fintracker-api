using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
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

        public Item[] Debits { get; set; }

        public decimal DebitTotal => Debits.Sum(debit => debit.Amount);

        public Item[] Credits { get; set; }

        public decimal CreditTotal => Credits.Sum(credit => credit.Amount);
    }
}
