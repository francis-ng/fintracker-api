namespace FinancialTrackerApi.Models
{
    public interface IDatabaseSettings
    {
        string UserCollectionName { get; set; }
        string LedgerCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }

    public class DatabaseSettings : IDatabaseSettings
    {
        public string UserCollectionName { get; set; }
        public string LedgerCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
