using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Npgsql;
using CryptoTrackApp.src.models;
using Microsoft.EntityFrameworkCore;
using CryptoTrackApp.src.utils;
using Microsoft.VisualBasic;
using Gdk;

namespace CryptoTrackApp.src.db
{
    public class PostgreRepository : IRepository
    {

        private Logger _logger = new Logger();
    
        public async Task<User?> Login(string email, string password)
        {

            _logger.Log($"[EXECUTE - Operation Login at PostgreRepository - Parameters [email: {email}; password: {password}]");

            using (var context = new CryptoTrackAppContext())
            {
                try
                {
                    return await context.Users.FirstOrDefaultAsync(u => u.Email == email);
                }
                catch (Exception error)
                {
                    _logger.Log($"[ERROR - Operation Login at PostgresRepository - Message: {error}]");
                    throw new Exception("Message");
                }
            }
        }
        public async Task<bool> ExistEmail(string email)
        {
            _logger.Log($"[EXECUTE - Operation ExistEmail at PostgreRepository - Parameters [email: {email}]");
        
            try 
            {
                using (var context = new CryptoTrackAppContext())
                {
                    return await context.Users.AnyAsync(u => u.Email == email);
                }
            }
            catch (Exception error)
            {
                _logger.Log($"[ERROR - Operation ExistEmail at PostgreRepository - Message: {error.Message}]");
                throw new Exception(error.Message);
            }
        }
        public async Task  AddUserAsync(User user)
        {

            _logger.Log($"[EXECUTE - Operation AddUserAsync at PostgreRepository - Parameters [user: {user.ToString()}]");

            using (var context = new CryptoTrackAppContext())
            {
                try
                {
                    await context.Users.AddAsync(user);
                    context.SaveChanges();
                }
                catch (Exception error)
                {
                    _logger.Log($"[ERROR - Operation AddUserAsync at PostgreRepository - Message: {error.Message}]");
                    throw new Exception("Error while adding a new user.");
                }
            }

        }

        public async Task AddSubscriptionAsync(Subscription sub)
        {

            _logger.Log($"[EXECUTE - Operation AssSubscriptionAsync at PostgreRepository - Parameters [sub: {sub.ToString()}]");
            using (var context = new CryptoTrackAppContext())
            {
                try
                {
                    await context.Subscriptions.AddAsync(sub);
                    context.SaveChanges();
                }
                catch (Exception error) 
                {
                    
                    _logger.Log($"[ERROR - Operation AddSubscriptionAsync at PostgreRepository - Message: {error.Message}]");
                    throw new Exception(error.Message);
                }
            }
        }

        public async Task<Subscription> GetSubscriptionAsync(Guid userId, string currencyId)
        {

            _logger.Log($"[EXECUTE - Operation GetSubscriptionAsync at PostgreRepository - Parameters [userId: {userId}; currencyId: {currencyId}]");

            using (var context = new CryptoTrackAppContext())
            {
                try
                {
                    _logger.Log($"Operation: GetSbuscriptionAsync; Parameters: userId: {userId}; currencyId: {currencyId}]");
                    return await context.Subscriptions.SingleAsync(sub => 
                        sub.UserId == userId && sub.CurrencyId == currencyId);
                }
                catch (Exception error) 
                {
                    _logger.Log($"[ERROR - Operation ExistEmail at PostgreRepository - Message: {error.Message}]");
                    throw new Exception(error.Message);
                }
            }
        }

        public async Task<List<Subscription>> GetSubscriptionAsync(Guid userId)
        {
            _logger.Log($"[EXECUTE - Operation GetSubscriptionAsync at PostgreRepository - Parameters [userId: {userId}]");

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
                    _logger.Log($"[ERROR - Operation GetSubscriptionAsync at PostgreRepository - Message: {error.Message}]");
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
            _logger.Log($"[EXECUTE - Operation GetUserAsync at PostgreRepository - Parameters [email: {email}]");
            using (var context = new CryptoTrackAppContext())
            {
                try
                {
                    return await context.Users.FirstOrDefaultAsync(user => user.Email == email);
                }

                catch(Exception error)
                {
                    _logger.Log($"[ERROR - Operation GetUserAsync at PostgreRepository - Message: {error.Message}]");
                    throw new Exception(error.Message);
                }
            }
        }

        public Task UpdateSubscriptionAsync(Subscription sub)
        {
            _logger.Log($"[EXECUTE - Operation UpdateSubscriptionAsync at PostgreRepository - Parameters [sub: {sub}]");
            throw new NotImplementedException();
        }

        public Task UpdateUserAsync(User user)
        {
            _logger.Log($"[EXECUTE - Operation UpdateUserAsync at PostgreRepository - Parameters [userId: {user}]");
            throw new NotImplementedException();
        }
    }


}
