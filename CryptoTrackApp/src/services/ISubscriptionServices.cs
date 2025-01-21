using System.Collections.Generic;
using System.Threading.Tasks;

namespace CryptoTrackApp.src.services
{
    public interface ISubscriptionServices
    {
        /// <summary>
        /// This method gets a list of the user's subscriptions from the database.
        ///Keys:
        ///- UserId, SubscriptionId, CurrencyId, NotificationThreshold, FollowDate, UnfollowDate.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>List of subscriptions or null (if there was an error).</returns>
        public Task<List<IDictionary<string, string>>?> GetSubscriptionsAsync(string userId);
        public Task<string> GetOneSubscriptionAsync(string userId, string currencyId);
        public void AddSubscriptionAsync(string userId, string currencyId);
        public void SetNotificationUmbral(string subscriptionId);
        public Task<bool> ToggleNotificationAsync(string subscriptionId);
        public Task<List<string>> GetFollowedCryptosIdsAsync(string userId);
    }
}