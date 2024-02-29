using System;
using System.Threading.Tasks;
using CryptoTrackApp.src.models;
using RestSharp;
using Newtonsoft.Json;
using System.Linq;

namespace CryptoTrackApp.src.db
{
    public class CoinApi : ICryptoApi
    {
        private string baseUrl = "https://api.coincap.io/v2";
        private RestClientOptions options;

        public CoinApi() {

            this.options = new RestClientOptions(baseUrl);
        }

        public async Task<Currency[]> GetCurrencies(string[] pIds)
        {
            string queryIds = pIds[0];

            foreach (var id in pIds.Skip(1)){
                queryIds += $",{id}";
            }

            using (RestClient client = new RestClient(options)) {

                RestRequest request = new RestRequest($"assets?ids={queryIds}");
                RestResponse response = await client.GetAsync(request);

                if (response.StatusCode != System.Net.HttpStatusCode.OK) {
                    throw new Exception($"Error: {response.StatusCode}. Description: {response.StatusDescription}");
                }

                return this.Deserialize<Currency[]>(response.Content);
            }
        }

        public async Task<Currency[]> GetCurrencies(int pOffset = 0, int pLimit = 100)
        {
            using (RestClient client = new RestClient(options)) {

                RestRequest request = new RestRequest($"assets?offset={pOffset}&limit={pLimit}");
                RestResponse response = await client.GetAsync(request);

                if (response.StatusCode != System.Net.HttpStatusCode.OK) {
                    throw new Exception($"Error: {response.StatusCode}. Description: {response.StatusDescription}");
                }

                return this.Deserialize<Currency[]>(response.Content);
            }
        }

        public async Task<Currency> GetCurrency(string pId)
        {
            using (RestClient client = new RestClient (options)) {

                RestRequest request = new RestRequest($"assets/{pId}");
                RestResponse response = await client.GetAsync(request);

                if (response.StatusCode != System.Net.HttpStatusCode.OK) {
                    throw new Exception($"Error: {response.StatusCode}. Description: {response.StatusDescription}");
                }

                return this.Deserialize<Currency>(response.Content);
            }
        }


// ------------------------ AUXILIAR METHODS --------------------------------------------------------
        private T Deserialize<T>(string pSerializedData) {

            var jsonData = JsonConvert.DeserializeObject<dynamic>(pSerializedData);
            var currencyDataJson = jsonData["data"].ToString();
            return JsonConvert.DeserializeObject<T>(currencyDataJson);
        }



    }
}