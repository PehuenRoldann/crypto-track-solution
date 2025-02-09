using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoTrackApp.src.db;
using CryptoTrackApp.src.models;
using CryptoTrackApp.src.utils;

namespace CryptoTrackApp.src.services
{
    public class SubscriptionServices : ISubscriptionServices
    {
        private IRepository repository = new PostgreRepository();
        private Logger _logger = new Logger();
        public async void AddSubscriptionAsync(string userId, string currencyId)
        {
            Subscription sub = new Subscription();
            sub.SubscriptionId = Guid.NewGuid();
            sub.CurrencyId = currencyId;
            sub.UserId = Guid.Parse(userId);
            sub.FollowDate = DateTime.UtcNow;
            await repository.AddSubscriptionAsync(sub);
        }

        public Task<string> GetOneSubscriptionAsync(string userId, string currencyId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UnfollowAsync (string userId, string currencyId) {
            _logger.Log($"[EXEC - Operation Unfollow at SubscriptionServices - Parameters: [currencyId {currencyId}]]");

            try {

                var guidUser = Guid.Parse(userId);
                var userSubs = await repository.GetSubscriptionAsync(guidUser);
                var sub = userSubs.FirstOrDefault(s => s.CurrencyId == currencyId);
                if (sub == null) {
                    throw new Exception("Subscription not found");
                }
                sub.UnfollowDate = DateTime.UtcNow;
                await this.repository.UpdateSubscriptionAsync(sub);
                _logger.Log($"[SUCCESS - Operation Unfollow at SubscriptionServices");
                return true;
            }
            catch(Exception ex) {
                _logger.Log($"[ERROR - Operation Unfollow at SubscriptionServices - Message: ${ex.Message}]");
                return false;
            }

        }

        public async Task<List<IDictionary<string, string>>?> GetSubscriptionsAsync(string userId)
        {
            _logger.Log($"[EXEC - Operation GetSubscriptionsAsync at SubscriptionServices - Parameters: [userId: {userId}]]");
            try
            {
                var subscriptionsData = new List<IDictionary<string, string>>();
                List<Subscription> data = await repository.GetSubscriptionAsync(Guid.Parse(userId));

                data = data.Where(s => s.UnfollowDate == null).ToList();

                foreach (var sub in data)
                {
                    subscriptionsData.Add(this.SubToDictionary(sub));
                }


                _logger.Log($"[SUCCESS - Operation GetSubscriptionsAsync at SubscriptionServices - Parameters: [userId: {userId}]]");
                return subscriptionsData;
            }
            catch (Exception error)
            {
                _logger.Log($"[ERROR - Operation GetSubscriptionsAsync at SubscriptionServices - Message: {error.Message}]");
                return null;
            }
        }

        public void SetNotificationUmbral(string subscriptionId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ToggleNotificationAsync(string subscriptionId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<string>> GetFollowedCryptosIdsAsync(string userId)
        {
            List<string> dataToReturn = new List<string>();
            List<Subscription> data = await repository.GetSubscriptionAsync(Guid.Parse(userId));

            foreach (Subscription sub in data)
            {
                dataToReturn.Add(sub.CurrencyId);
            }

            return dataToReturn;
        }


        private IDictionary<string, string> SubToDictionary(Subscription sub)
        {
            var subscriptionsData = new Dictionary<string, string>();

            foreach (var property in sub.GetType().GetProperties())
            {
                if (property.GetValue(sub) == null && property.Name != "User")
                {
                    subscriptionsData.Add(property.Name, "");
                }
                else if (property.Name != "User")
                {
                    subscriptionsData.Add(property.Name, property.GetValue(sub).ToString());
                }
            }

            return subscriptionsData;
        }

    }
}