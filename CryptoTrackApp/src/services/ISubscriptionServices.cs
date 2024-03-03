using System.Collections.Generic;
using System.Threading.Tasks;

namespace CryptoTrackApp.src.services
{
    public interface ISubscriptionServices
    {
        public Task<List<IDictionary<string, string>>> GetSubscriptionsAsync(string userId);
        public Task<string> GetOneSubscriptionAsync(string userId, string currencyId);
        public void AddSubscriptionAsync(string userId, string currencyId);
        public void SetNotificationUmbral(string subscriptionId);
        public Task<bool> ToggleNotificationAsync(string subscriptionId);
        public Task<List<string>> GetFollowedCryptosIdsAsync(string userId);
    }
}