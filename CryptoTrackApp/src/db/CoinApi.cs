using System;
using System.IO;
using System.Threading.Tasks;
using CryptoTrackApp.src.models;
using RestSharp;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using CryptoTrackApp.src.utils;

namespace CryptoTrackApp.src.db
{
    public class CoinApi : ICryptoApi
    {
        // private string baseUrl = "https://api.coincap.io/v2";
        private string baseUrl = "";
        private RestClientOptions options;
        private Logger _logger = new Logger();

        public CoinApi() {

            var configService = new ConfigService(Config.JsonConfArrPath);
			baseUrl = configService.GetString(EnvNames.CoinCapApi)!;
            this.options = new RestClientOptions(baseUrl);
        }

// ------------------------ AUXILIAR METHODS --------------------------------------------------------
        private T Deserialize<T>(string pSerializedData) {

            var jsonData = JsonConvert.DeserializeObject<dynamic>(pSerializedData);
            var currencyDataJson = jsonData["data"].ToString();
            return JsonConvert.DeserializeObject<T>(currencyDataJson);
        }



// ----------------------- INTERFACE IMPLEMENTATIONS ------------------------------------------------------------

        
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
                    _logger.Log($"[ERROR - Operation: GetCurrencies at CoinApi - Message: {error.Message}]");
                    throw error;
                } 

                
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

        public async Task<List<(DateTime, double)>> GetHistory(string pId)
        {
            using (RestClient client = new RestClient (options))
            {

                RestRequest request = new RestRequest($"assets/{pId}/history?interval=d1");
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

                    double historyValue = Double.Parse( item["priceUsd"] );

                    historyValues.Add((historyDate, historyValue));
                }

                return historyValues;
            }
            
        }
    }

}