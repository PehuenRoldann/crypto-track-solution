using System;
using CryptoTrackApp.src.services;
using CryptoTrackApp.src.models;
using System.Threading.Tasks;
using Xunit;
using System.Collections.Generic;
using Xunit.Sdk;
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

            IDictionary<string, object>? currencyData = await service.GetCurrency(wantedId);

            Assert.Equal(wantedId, currencyData["Id"]);
        }

        [Fact]
        public async void GetCurrency_null () 
        {
            string wantedId = "peroniacoin";

            ICurrencyServices service = new CurrencyServices();

            IDictionary<string, object>? currencyData = await service.GetCurrency(wantedId);

            Assert.Null(currencyData);

        }

        [Fact]
        public async void GetCurrencies()
        {

            ICurrencyServices service = new CurrencyServices();
            IDictionary<string, object>[] currencyData = await service.GetCurrencies(offset:50, limit:100);
            Assert.Equal("raydium", currencyData[95]["Id"]);  

        }

        /* [Fact]
        public async void GetCurrenciesWithFilter () {

            ICurrencyServices service = new CurrencyServices();
            string[] ids = new string[] {"bitcoin", "zilliqa", "usd-coin", "ethereum", "cardano"};
            IDictionary<string, object>[]? result = await service.GetCurrencies(ids);
            Console.WriteLine("~~~~~~~~~~Cantidad encontrados: " + result.Length);
            foreach (var item in result) {
                Console.WriteLine(item["Name"]);
            } 
            Assert.True(ids.Contains(result[1]["Id"]));

        } */


        /* [Fact]
        public async void GetCurrencyDetail_true(){

        } */
    }
}