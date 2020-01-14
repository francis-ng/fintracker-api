using FinancialTrackerApi.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Linq;

namespace FinancialTrackerApi.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;
        private readonly ILogger _logger;

        public UserService(IDatabaseSettings settings, ILogger<UserService> logger) {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<User>(settings.UserCollectionName);
            _logger = logger;
        }

        public User Get(string name) =>
            _users.Find(user => user.UserName == name).FirstOrDefault();

        public User Add(User user) {
            _users.InsertOne(user);
            return user;
        }

        public void Update(string id, User user) =>
            _users.ReplaceOne(user => user.Id == id, user);

        public void Delete(string id) =>
            _users.DeleteOne(user => user.Id == id);
    }
}