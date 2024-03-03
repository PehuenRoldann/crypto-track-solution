using System;
using CryptoTrackApp.src.services;
using Xunit;
using System.Collections.Generic;
using CryptoTrackApp.src.models;
using System.Linq;

namespace CryptoTrackTest
{
    public class CurrencyServicesTest
    {
        [Fact]
        public async void GetCurrency_true()
        {
            string wantedId = "ethereum";

            ICurrencyServices service = new CurrencyServices();

            IDictionary<string, string> currency = await service.GetCurrency(wantedId);

            Assert.Equal(wantedId, currency["Id"]);
        }

        [Fact]
        public async void GetCurrency_null () 
        {
            string wantedId = "argentum-coin";

            ICurrencyServices service = new CurrencyServices();

            await Assert.ThrowsAsync<Exception>(async () => await service.GetCurrency(wantedId));
        }

        [Fact]
        public async void GetCurrencies()
        {

            ICurrencyServices service = new CurrencyServices();
            IDictionary<string, string>[] currencyData = await service.GetCurrencies(offset:50, limit:100);
            Console.WriteLine($"Cantidad: {currencyData.Length}");
            Assert.Equal("chiliz", currencyData[4]["Id"]);

        }

       [Fact]
        public async void GetCurrenciesOnlySelectedIds() {

            ICurrencyServices service = new CurrencyServices();
            string[] ids = new string[] {"bitcoin", "zilliqa", "usd-coin", "ethereum", "cardano"};
            IDictionary<string, string>[] currencies = await service.GetCurrencies(ids);
            Assert.True(ids.Contains(currencies[3]["Id"]));

        }


        [Fact]
        public async void GetCurrenciesOnlySelectedIds_OneNotExist() {

            ICurrencyServices service = new CurrencyServices();
            string[] ids = new string[] {"bitcoin", "zilliqa", "usd-coin", "argentum-coin", "cardano"};
            IDictionary<string, string>[] currencies = await service.GetCurrencies(ids);
            Assert.Equal(4, currencies.Length);

        }


        /* [Fact]
        public async void GetHistory()
        {
            string wantedId = "casper";
            double expectedValue = Double.Parse("0.03875038988142513354");
            ICurrencyServices service = new CurrencyServices();
            IList<(DateTime, double)> historyValues = await service.GetHistory(wantedId);
            
            Assert.Equal(expectedValue, historyValues[179].Item2);

        }  */
    }
}