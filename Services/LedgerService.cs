using FinancialTrackerApi.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<MonthlyLedger> Find(string id) =>
            await _ledgers.FindAsync(ledger => ledger.Id == id).GetAwaiter().GetResult().FirstOrDefaultAsync();

        public async Task<List<MonthlyLedger>> Get(string owner) =>
            await _ledgers.FindAsync(ledger => ledger.Owner == owner).GetAwaiter().GetResult().ToListAsync();

        public async Task<MonthlyLedger> Get(string owner, int year, int month, string type) =>
            await _ledgers.FindAsync(ledger => ledger.Owner == owner && ledger.Year == year && ledger.Month == month && ledger.Type == type).GetAwaiter().GetResult().FirstOrDefaultAsync();

        public async Task<MonthlyLedger> Add(MonthlyLedger ledger) {
            await _ledgers.InsertOneAsync(ledger);
            return ledger;
        }

        public async Task Update(string id, MonthlyLedger ledger) =>
            await _ledgers.ReplaceOneAsync(ledger => ledger.Id == id, ledger);

        public async Task Delete(string id) =>
            await _ledgers.DeleteOneAsync(ledger => ledger.Id == id);
    }
}