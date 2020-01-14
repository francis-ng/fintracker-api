using FinancialTrackerApi.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace FinancialTrackerApi.Services
{
    public class LedgerService
    {
        private readonly IMongoCollection<MonthlyLedger> _ledgers;
        private readonly ILogger _logger;

        public LedgerService(IDatabaseSettings settings, ILogger<LedgerService> logger) {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _ledgers = database.GetCollection<MonthlyLedger>(settings.LedgerCollectionName);
            _logger = logger;
        }

        public MonthlyLedger Find(string id) =>
            _ledgers.Find(ledger => ledger.Id == id).FirstOrDefault();

        public List<MonthlyLedger> Get(string owner) =>
            _ledgers.Find(ledger => ledger.Owner == owner).ToList();

        public MonthlyLedger Get(string owner, int year, int month) =>
            _ledgers.Find(ledger => ledger.Owner == owner && ledger.Year == year && ledger.Month == month).FirstOrDefault();

        public MonthlyLedger Add(MonthlyLedger ledger) {
            _ledgers.InsertOne(ledger);
            return ledger;
        }

        public void Update(string id, MonthlyLedger ledger) =>
            _ledgers.ReplaceOne(ledger => ledger.Id == id, ledger);

        public void Delete(string id) =>
            _ledgers.DeleteOne(ledger => ledger.Id == id);
    }
}