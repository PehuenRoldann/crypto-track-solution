using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CryptoTrackApp.src.models;
using RestSharp;
using Newtonsoft.Json;
using System.Text.Json.Nodes;


namespace CryptoTrackApp.src.services
{
    public class CurrencyServices : ICurrencyServices
    {
        private string baseUrl = "https://api.coincap.io/v2/";
        private RestClientOptions options;

        public CurrencyServices () {

            this.options = new RestClientOptions(baseUrl);
        }

        public async Task<Currency?> GetCurrency(string pCurrencyId)
        {

            using (var client = new RestClient(options)) {
                var request = new RestRequest($"{baseUrl}assets/{pCurrencyId}", Method.Get);
                RestResponse response = await client.GetAsync(request);

               try {

                    if (response.StatusCode != System.Net.HttpStatusCode.OK) {
                        Console.WriteLine($">>>> Query Failure: {response.StatusCode.ToString()},{response.StatusDescription}");
                        return null;
                    }
                    
                    var jsonData = JsonConvert.DeserializeObject<dynamic>(response.Content);
                    var currencyDataJson = jsonData["data"].ToString();
                    return JsonConvert.DeserializeObject<Currency>(currencyDataJson);
               }
               catch (Exception error) {
                Console.WriteLine($"Unexpected Error: {error.Message}");
                return null;
               }
            }
        }

        public IDictionary<int, double> GetHistory(string pCurrencyId, DateTime pFrom, DateTime pTo)
        {
            throw new NotImplementedException();
        }
    }
}