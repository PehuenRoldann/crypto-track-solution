using System;
using CryptoTrackApp.src.services;
using CryptoTrackApp.src.models;
using System.Threading.Tasks;
using Xunit;

namespace CryptoTrackTest
{
    public class CurrencyServicesTest
    {
        [Fact]
        public async void GetCurrency_true()
        {
            string wantedId = "tether";

            ICurrencyServices service = new CurrencyServices();

            Currency? currency = await service.GetCurrency(wantedId);

            Assert.Equal(wantedId, currency.Value.Id);
        }
    }
}