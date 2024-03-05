using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using CryptoTrackApp.src.models;

namespace CryptoTrackApp.src.db
{

    public interface IRepository
    {

        /* object[] AddUser(User pUser); */
        public Task<User?> Login(string pEmail, string pPassword);
        public Task<bool> ExistEmail(string pEmail);
        public Task AddSubscriptionAsync(Subscription sub);
        public Task AddUserAsync(User user);
        public Task<Subscription> GetSubscriptionAsync(Guid userId, string currencyId);
        public Task<List<Subscription>> GetSubscriptionAsync(Guid userId);
        public Task<User?> GetUserAsync(Guid userId);
        public Task<User?> GetUserAsync(string email);
        public Task UpdateSubscriptionAsync(Subscription sub);
        public Task UpdateUserAsync(User user);
    }
}
