using System.Collections.Generic;
using System.Threading.Tasks;
using CryptoTrackApp.src.models;

namespace CryptoTrackApp.src.services
{
    public interface ISubscriptionServices
    {
        /// <summary>
        /// This method gets a list of the user's active subscriptions from the database.
        ///Keys:
        ///- UserId, SubscriptionId, CurrencyId, NotificationThreshold, FollowDate, UnfollowDate.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>List of subscriptions or null (if there was an error).</returns>
        public Task<List<IDictionary<string, string>>?> GetActiveSubscriptionsListAsync(string userId);

        /// <summary>
        /// Adds a subscription or updates the follow date from a previus subscription.
        /// </summary>
        /// <param name="userId">User's id</param>
        /// <param name="currencyId">Currency's id</param>
        public void AddSubscriptionAsync(string userId, string currencyId);

        /// <summary>
        /// Sets the notification treshold for a subscription.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="currencyId"></param>
        /// <param name="notificationUmbral"></param>
        /// <returns>
        /// True if the operations finished successfully, False if there was an error.
        /// </returns>
        public Task<bool> SetNotificationTreshold(string userId, string currencyId, float notificationUmbral);

        /// <summary>
        /// Returns the followed cryptos Ids for an user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>List of ids</returns>
        public Task<List<string>> GetFollowedCryptosIdsAsync(string userId);

        /// <summary>
        /// Sets the unfollow date for a subscription as the actual date.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="currencyId"></param>
        /// <returns>
        /// True if the operations finished successfully, False if there was an error.
        /// </returns>
        public Task<bool> UnfollowAsync(string userId, string currencyId);
    }
}