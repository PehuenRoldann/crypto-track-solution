using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Npgsql;
using CryptoTrackApp.src.models;
using Microsoft.EntityFrameworkCore;

namespace CryptoTrackApp.src.db
{
    public class PostgreRepository : IRepository
    {
    
        public async Task<User?> Login(string pEmail, string pPassword)
        {

            using (var context = new CryptoTrackAppContext())
            {
                
                try
                {
                    return await context.Users.FirstOrDefaultAsync(u => u.Email == pEmail);
                }
                catch (Exception error)
                {
                    Logger.LogErrorAsync("Error while login", error);
                    throw new Exception("Message");
                }
            }
        }
        public async Task<bool> ExistEmail(string pEmail)
        {
        
            try 
            {
                using (var context = new CryptoTrackAppContext())
                {
                    return await context.Users.AnyAsync(u => u.Email == pEmail);
                }
            }
            catch (Exception error)
            {
                throw new Exception(error.Message);
            }
        }
        public async Task  AddUserAsync(User pUser)
        {
            using (var context = new CryptoTrackAppContext())
            {
                try
                {
                    await context.Users.AddAsync(pUser);
                    context.SaveChanges();
                }
                catch (Exception error)
                {
                    
                    Logger.LogErrorAsync("Error while adding a new user.", error);
                    throw new Exception("Error while adding a new user.");
                }
            }

        }

        public async Task AddSubscriptionAsync(Subscription sub)
        {
            throw new NotImplementedException();
        }

        public async Task<Subscription> GetSubscriptionAsync(Guid userId, string currencyId)
        {
            using (var context = new CryptoTrackAppContext())
            {
                try
                {
                    return await context.Subscriptions.SingleAsync(sub => 
                        sub.UserId == userId && sub.CurrencyId == currencyId);
                }
                catch (Exception error) 
                {
                    await Logger.LogErrorAsync("Error while getting subscriptions", error);
                    throw new Exception(error.Message);
                }
            }
        }

        public async Task<List<Subscription>> GetSubscriptionAsync(Guid userId)
        {
            using (var context = new CryptoTrackAppContext())
            {
                try
                {
                    return await context.Subscriptions
                        .Where<Subscription>(sub => sub.UserId == userId)
                        .ToListAsync<Subscription>();
                }
                catch (Exception error)
                {
                    await Logger.LogErrorAsync("Error while getting subscriptions", error);
                    throw new Exception(error.Message);
                }
            }
        }

        public Task<User?> GetUserAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task<User?> GetUserAsync(string email)
        {
            using (var context = new CryptoTrackAppContext())
            {
                try
                {
                    return await context.Users.FirstOrDefaultAsync(user => user.Email == email);
                }

                catch(Exception error)
                {
                    Logger.LogErrorAsync("Error while getting user.", error);
                    throw new Exception(error.Message);
                }
            }
        }

        public Task UpdateSubscriptionAsync(Subscription sub)
        {
            throw new NotImplementedException();
        }

        public Task UpdateUserAsync(User user)
        {
            throw new NotImplementedException();
        }
    }


}
