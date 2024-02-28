using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CryptoTrackApp.src.models;
using RestSharp;
using Newtonsoft.Json;
using System.Linq;


namespace CryptoTrackApp.src.services
{
    public class CurrencyServices : ICurrencyServices
    {
        private string baseUrl = "https://api.coincap.io/v2/";
        private RestClientOptions options;

        public CurrencyServices () {

            this.options = new RestClientOptions(baseUrl);
        }

        public async Task<IDictionary<string, object>? > GetCurrency(string pCurrencyId)
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
                    Currency currency = JsonConvert.DeserializeObject<Currency>(currencyDataJson);

                    return this.CurrencyToDictionary(currency);
               }
               catch (Exception error) {
                Console.WriteLine($"Unexpected Error: {error.Message}");
                return null;
               }
            }
        }

        /// <summary>
        /// Gets all the currencies in the database.
        /// </summary>
        /// <returns>
        /// An Array with dictionaries that contains currency info.
        /// </returns>
        public async Task<IDictionary<string, object>[]?> GetCurrencies(int pOffset = 0, int pLimit = 100)
        {
           using (var client = new RestClient(options)) {

                var request = new RestRequest($"{baseUrl}assets?offset={pOffset}&limit={pLimit}", Method.Get);
                RestResponse response = await client.GetAsync(request);

                try{
                    if (response.StatusCode != System.Net.HttpStatusCode.OK) {
                        Console.WriteLine($">>>> Query Failure: {response.StatusCode.ToString()},{response.StatusDescription}");
                        return null;
                    }

                    var jsonData = JsonConvert.DeserializeObject<dynamic>(response.Content);
                    var currencyDataJson = jsonData["data"].ToString();
                    Currency[] currencies = JsonConvert.DeserializeObject<Currency[]>(currencyDataJson);

                    return currencies.Select(item => {

                        return this.CurrencyToDictionary(item);

                    }).ToArray();
                }
                catch (Exception error) {
                    Console.WriteLine($"Unexpected Error: {error.Message}");
                    return null;
                }

           }
        }
        /* public async Task<IDictionary<string, object>[]?> GetCurrencies() {

            using (var client = new RestClient(options)) {

                var request = new RestRequest($"{baseUrl}assets/", Method.Get);
                RestResponse response = await client.GetAsync(request);

                try{
                    if (response.StatusCode != System.Net.HttpStatusCode.OK) {
                        Console.WriteLine($">>>> Query Failure: {response.StatusCode.ToString()},{response.StatusDescription}");
                        return null;
                    }

                    var jsonData = JsonConvert.DeserializeObject<dynamic>(response.Content);
                    var currencyDataJson = jsonData["data"].ToString();
                    Currency[] currencies = JsonConvert.DeserializeObject<Currency[]>(currencyDataJson);

                    return currencies.Select(item => {

                        return this.CurrencyToDictionary(item);

                    }).ToArray();
                }
                catch (Exception error) {
                    Console.WriteLine($"Unexpected Error: {error.Message}");
                    return null;
                }
                
            }
        } */

        /// <summary>
        /// Gets all the curriencies that matches the ids passed as parameters.
        /// </summary>
        /// <param name="pIds">Ids to match.</param>
        /// <returns>
        /// An Array with dictionaries that contains currency information.
        /// </returns>
        public async Task<IDictionary<string, object>[]?> GetCurrencies (string[] pIds) {

            /* IDictionary<string, object>[]? curriencies = await this.GetCurrencies();

            if (curriencies == null) {
                return null;
            }

            return curriencies.Where(item => pIds.Contains(item["Id"]) ).ToArray(); */
            throw new NotImplementedException();
        } 


        public IDictionary<int, double> GetHistory(string pCurrencyId, DateTime pFrom, DateTime pTo)
        {
            throw new NotImplementedException();
        }



        private IDictionary<string, object> CurrencyToDictionary (Currency pCurrency) {
            
            IDictionary<string, object> currencyData = new Dictionary<string, object>();

            foreach (var property in pCurrency.GetType().GetProperties()) {

                currencyData.Add(property.Name, property.GetValue(pCurrency));
            }

            return currencyData;
        }

        

    }
}