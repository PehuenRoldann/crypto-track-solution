using System;
using System.IO;
using System.Threading.Tasks;
using CryptoTrackApp.src.models;
using RestSharp;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;

namespace CryptoTrackApp.src.db
{
    public class CoinApi : ICryptoApi
    {
        // private string baseUrl = "https://api.coincap.io/v2";
        private string baseUrl = "";
        private RestClientOptions options;

        public CoinApi() {

            string configRaw = File.ReadAllText("./src/conf.json");
			var configJsonData = JsonConvert.DeserializeObject<dynamic>(configRaw);
			baseUrl = configJsonData["coin_api_url"].ToString();
            this.options = new RestClientOptions(baseUrl);
        }

// ------------------------ AUXILIAR METHODS --------------------------------------------------------
        private T Deserialize<T>(string pSerializedData) {

            var jsonData = JsonConvert.DeserializeObject<dynamic>(pSerializedData);
            var currencyDataJson = jsonData["data"].ToString();
            return JsonConvert.DeserializeObject<T>(currencyDataJson);
        }



// ----------------------- INTERFACE IMPLEMENTATIONS ------------------------------------------------------------

        /// <summary>
        /// Gets the selected currency from the Coin Api service.
        /// </summary>
        /// <param name="pIds">Currency's Id we want to query.</param>
        /// <returns>Currency</returns>
        /// <exception cref="Exception">
        /// Throws an exception when the Http status of the response is different from Ok (200).
        /// </exception>
        public async Task<Currency[]> GetCurrencies(string[] pIds)
        {
            if (pIds.Length == 0) {
                throw new Exception ("Empty ids string.");
            }

            string queryIds = pIds[0];

            foreach (var id in pIds.Skip(1)){
                queryIds += $",{id}";
            }

            using (RestClient client = new RestClient(options)) {

                try {

                    RestRequest request = new RestRequest($"assets?ids={queryIds}");
                    RestResponse response = await client.GetAsync(request);

                    if (response.StatusCode != System.Net.HttpStatusCode.OK) {
                        throw new Exception($"Error: {response.StatusCode}. Description: {response.StatusDescription}");
                    }

                    return this.Deserialize<Currency[]>(response.Content);

                } catch (Exception error) {
                    Logger.LogErrorAsync("Error at CoinApi class", error);
                    throw error;
                } 

                
            }
        }

        /// <summary>
        /// Gets a number of "pLimit" currencies form the Coin Api.
        /// By default the currencies are ordered by rank, use "pOffsett" to skip determinated number.
        /// </summary>
        /// <param name="pOffset">Number of currencies to query.</param>
        /// <param name="pLimit">Number of currencies to skip.</param>
        /// <returns>
        /// An array with currencies from Coin Api.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws an exception when the Http status of the response is different from Ok (200).
        /// <exception
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

        /// <summary>
        /// Gets the currencies with the given ids from Coin Api.
        /// </summary>
        /// <param name="pId">Ids to query for.</param>
        /// <returns>
        /// An array with currencies from Coin Api.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws an exception when the Http status of the response is different from Ok (200).
        /// <exception
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

        public async Task<List<(DateTime, double)>> GetHistory(string pId)
        {
            using (RestClient client = new RestClient (options))
            {

                RestRequest request = new RestRequest($"assets/{pId}/history?interval=h6");
                RestResponse response = await client.GetAsync(request);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception($"Error: {response.StatusCode}. Description: {response.StatusDescription}");
                }

                Dictionary<string, string>[] data = this.Deserialize<Dictionary<string, string>[]>(response.Content);
                List<(DateTime, double)> historyValues = new List<(DateTime, double)>();

                foreach (Dictionary<string, string> item in data)
                {
                    DateTime historyDate = new DateTime(1970, 1, 1, 0, 0 ,0 ,0, DateTimeKind.Utc);
                    historyDate = historyDate.AddMilliseconds(Double.Parse(item["time"])).ToLocalTime();

                    double historyValue = Double.Parse(item["priceUsd"]);

                    historyValues.Add((historyDate, historyValue));
                }

                return historyValues;
            }
            
        }
    }

}