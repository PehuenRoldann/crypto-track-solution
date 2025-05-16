using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoTrackApp.src.db;
using CryptoTrackApp.src.models;
using CryptoTrackApp.src.utils;
using Gdk;

namespace CryptoTrackApp.src.services
{
    public class SubscriptionServices : ISubscriptionServices
    {
        private IRepository repository = new PostgreRepository();
        private Logger _logger = new Logger();
        
        // ----------------- IMPLEMENTATIONS -----------------------------------------------------
        public async void AddSubscriptionAsync(string pUserId, string pCurrencyId)
        {
            Guid userId = Guid.Parse(pUserId);
            Subscription? oldSub = await repository.GetSubscriptionAsync(userId, pCurrencyId);
            if (oldSub != null)
            {
                oldSub.FollowDate = DateTime.UtcNow;
                await repository.UpdateSubscriptionAsync(oldSub);
            }
            else
            {
                Subscription sub = new Subscription();
                sub.SubscriptionId = Guid.NewGuid();
                sub.CurrencyId = pCurrencyId;
                sub.UserId = userId;
                sub.FollowDate = DateTime.UtcNow;
                await repository.AddSubscriptionAsync(sub);
            }

        }

        public async Task<bool> UnfollowAsync (string pUserId, string pCurrencyId) {
            _logger.Log($"[EXEC - Operation Unfollow at SubscriptionServices - Parameters: [currencyId {pCurrencyId}]]");

            try {

                var guidUser = Guid.Parse(pUserId);
                var userSubs = await repository.GetSubscriptionsListAsync(guidUser);
                var sub = userSubs.FirstOrDefault(s => s.CurrencyId == pCurrencyId);
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

        public async Task<List<IDictionary<string, string>>?> GetActiveSubscriptionsListAsync(string pUserId)
        {
            _logger.Log($"[EXEC - Operation GetSubscriptionsListAsync at SubscriptionServices - Parameters: [userId: {pUserId}]]");
            try
            {
                var subscriptionsData = new List<IDictionary<string, string>>();
                List<Subscription> data = await repository.GetSubscriptionsListAsync(Guid.Parse(pUserId));

                data = data.Where(s => s.UnfollowDate == null|| s.FollowDate > s.UnfollowDate).ToList();

                foreach (var sub in data)
                {
                    subscriptionsData.Add(this.SubToDictionary(sub));
                }


                _logger.Log($"[SUCCESS - Operation GetSubscriptionsListAsync at SubscriptionServices - Parameters: [userId: {pUserId}]]");
                return subscriptionsData;
            }
            catch (Exception error)
            {
                _logger.Log($"[ERROR - Operation GetSubscriptionsListAsync at SubscriptionServices - Message: {error.Message}]");
                return null;
            }
        }

        public async Task<bool> SetNotificationTreshold(string pUserId, string pCurrencyId, float pNotificationUmbral)
        {
             _logger.Log("[EXECUTE - Operation SetNotificationUmbral at SubscriptionServices - Parameters:" + 
                $"[userId: {pUserId}; currencyId: {pCurrencyId}; notificationUmbral: {pNotificationUmbral}]]");
            try {
                List<Subscription> subs = await repository.GetSubscriptionsListAsync(Guid.Parse(pUserId));
                var sub = subs.Where(s =>
                    s.UserId == Guid.Parse(pUserId) &&
                    s.CurrencyId == pCurrencyId &&
                    (s.UnfollowDate == null || s.FollowDate > s.UnfollowDate)
                ).First();
                sub.NotificationUmbral = pNotificationUmbral;
                await repository.UpdateSubscriptionAsync(sub);
                _logger.Log($"[SUCCESS - Operation SetNotificationUmbral at SubscriptionServices]");
                return true;
            }
            catch (Exception ex) {
                _logger.Log($"[ERROR - Operation SetNotificationUmbral at SubscriptionServices - Message: {ex.Message}]");
                return false;
            }
            
        }

        public async Task<List<string>> GetFollowedCryptosIdsAsync(string pUserId)
        {
            _logger.Log($"[EXECUTE - Operation GetFollowedCryptosIdsAsync at SubscriptionServices - userId: {pUserId}]");
            List<string> dataToReturn = new List<string>();
            try
            {
                List<Subscription> data = await repository.GetSubscriptionsListAsync(Guid.Parse(pUserId));

                foreach (Subscription sub in data)
                {
                    if (sub.UnfollowDate == null || sub.FollowDate > sub.UnfollowDate) dataToReturn.Add(sub.CurrencyId);
                }

            }
            catch (Exception error)
            {
                _logger.Log($"[ERROR - GetFollowedCryptosIdsAsync at SbuscriptionService - message: {error.Message}]");
            }

            return dataToReturn;
        }


        // ------------------- AUXILIAR ----------------------------------------------
        
        /// <summary>
        /// Returns a dictionary withe keys and values for a subscription.
        /// </summary>
        /// <param name="pSub">Subscription</param>
        /// <returns>Key value pairs with Subscription attributes as keys</returns>
        private IDictionary<string, string> SubToDictionary(Subscription pSub)
        {
            var subscriptionsData = new Dictionary<string, string>();

            foreach (var property in pSub.GetType().GetProperties())
            {
                if (property.GetValue(pSub) == null && property.Name != "User")
                {
                    subscriptionsData.Add(property.Name, "");
                }
                else if (property.Name != "User")
                {
                    subscriptionsData.Add(property.Name, property.GetValue(pSub).ToString());
                }
            }

            return subscriptionsData;
        }

    }
}