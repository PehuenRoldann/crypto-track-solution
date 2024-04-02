using System;
using CryptoTrackApp.src.services;
using Xunit;
using System.Collections.Generic;

namespace CryptoTrackTest
{
    public class SubscriptionServicesTest
    {

        [Fact]
        public async void AddSubscriptionTest()
        {
            string userId = "7a43853d-b414-4432-b00c-5fd18f77abf6";
            string currencyId = "dai";
            ISubscriptionServices services = new SubscriptionServices();
            services.AddSubscriptionAsync(userId, currencyId);
        }

        [Fact]
        public async void GetSubscriptionsTest ()
        {
            string userId = "4d266202-d63e-4caf-a87f-6ef56e0dd1b6";
            ISubscriptionServices services = new SubscriptionServices();
            List<IDictionary<string, string>> data = await services.GetSubscriptionsAsync(userId);
            Assert.Equal(3, data.Count);
        }

        [Fact]
        public async void GetFollowedCryptosIdsTest ()
        {
            string userId = "4d266202-d63e-4caf-a87f-6ef56e0dd1b6";
            ISubscriptionServices services = new SubscriptionServices();
            List<string> data = await services.GetFollowedCryptosIdsAsync(userId);
            Assert.Equal(3, data.Count);
        }
    }
}