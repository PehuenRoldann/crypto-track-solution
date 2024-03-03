using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoTrackApp.src.db;
using CryptoTrackApp.src.models;

namespace CryptoTrackApp.src.services
{
    public class SubscriptionServices : ISubscriptionServices
    {
        private IRepository repository = new PostgreRepository();
        public void AddSubscriptionAsync(string userId, string currencyId)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetOneSubscriptionAsync(string userId, string currencyId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<IDictionary<string, string>>> GetSubscriptionsAsync(string userId)
        {
            try
            {
                var subscriptionsData = new List<IDictionary<string, string>>();
                List<Subscription> data = await repository.GetSubscriptionAsync(Guid.Parse(userId));

                foreach (var sub in data)
                {
                    subscriptionsData.Add(this.SubToDictionary(sub));
                }

                return subscriptionsData;
            }
            catch (Exception error)
            {
                throw new Exception(error.Message);
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
                if (property.GetValue(sub) == null)
                {
                    subscriptionsData.Add(property.Name, "");
                }
                else
                {
                    subscriptionsData.Add(property.Name, property.GetValue(sub).ToString());
                }
            }

            return subscriptionsData;
        }
    }
}