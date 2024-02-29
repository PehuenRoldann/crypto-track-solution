using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoTrackApp.src.db;
using CryptoTrackApp.src.models;



namespace CryptoTrackApp.src.services
{
    public class CurrencyServices : ICurrencyServices
    {
        private ICryptoApi cryptoApi;

        public CurrencyServices() {

            this.cryptoApi = new CoinApi();
        }

// ------------------- INTERFACE IMPLEMENTATIONS -------------------------------------------------

        /// <summary>
        /// Returns a currency with the given Id.
        /// </summary>
        /// <param name="pCurrencyId">Id of the currency we want to return</param>
        /// <returns>
        /// IDictionary<string, string> with the currency information.
        /// </returns>
        public async Task<IDictionary<string, string>> GetCurrency(string pCurrencyId)
        {

            try {
                Currency currency = await this.cryptoApi.GetCurrency(pCurrencyId);
                return this.CurrencyToDictionary(currency);
            }
            catch (Exception error) {

                Console.WriteLine($"CurrencyService-Error: {error.Message}");
                throw new Exception(error.Message);
            }

        }

        /// <summary>
        /// Returs a collection with the information of crypto currencies.
        /// </summary>
        /// <param name="pOffset">The number of currencies we have already queried before.</param>
        /// <param name="pLimit">The number of currencies we want to receive.</param>
        /// <returns>
        /// A collection if Dictionary<string, string> with the currency information as an Array.
        /// </returns>
        /// <exception cref="Exception">
        /// If the query fails for some reason.
        /// </exception>
        public async Task<IDictionary<string, string>[]> GetCurrencies(int pOffset = 0, int pLimit = 100)
        {
            try {

                Currency[] currencyData = await this.cryptoApi.GetCurrencies(offset: pOffset, limit: pLimit);
                return this.CurrencyToDictionary(currencyData);
            }
            catch (Exception error) {
                Console.WriteLine($"CurrencyService-Error: {error.Message}");
                throw new Exception(error.Message);
            }
        }
/*
        /// <summary>
        /// Gets all the curriencies that matches the ids passed as parameters.
        /// </summary>
        /// <param name="pIds">Ids to match.</param>
        /// <returns>
        /// An Array with dictionaries that contains currency information.
        /// </returns>
        public async Task<IDictionary<string, object>> GetCurrencies (string[] pIds) {
            
            String idsParam = pIds[0];
            for (var i = 1; i < pIds.Length; i++) {

                idsParam += $",{pIds[i]}";
            }

            IDictionary<string, object> result = new Dictionary<string, object>();
            result.Add("status", null);
            result.Add("data", null);

            using (var client = new RestClient(options)) {

                

                var request = new RestRequest($"assets?ids={idsParam}");
                RestResponse response = await client.GetAsync(request);
                
                try {
                    result["status"] = response.StatusCode.ToString();

                    if (response.StatusCode != System.Net.HttpStatusCode.OK){
                        Console.WriteLine($"Petition fail: {response.StatusDescription}");   
                        result["data"] = response.StatusDescription;
                    }

                    else {

                        result["data"] = this.FromResponseToDictionaries(response);

                    }

                }
                catch (Exception error) {
                    Console.WriteLine($"Unexpected Error: {error.Message}");
                    result["data"] = error.Message;
                }
            }

            return result;
            
        } 


        public Task<IDictionary<int, double>> GetHistory(string pCurrencyId)
        {
            throw new NotImplementedException();
        }
 */

//----------------------- AUXILIAR METHODS ----------------------------------------------------------

        private IDictionary<string, string> CurrencyToDictionary (Currency pCurrency) {
            
            IDictionary<string, string> currencyData = new Dictionary<string, string>();

            foreach (var property in pCurrency.GetType().GetProperties()) {

                if (property.GetValue(pCurrency) == null) {
                    currencyData.Add(property.Name, "");
                }
                else {
                    currencyData.Add(property.Name, property.GetValue(pCurrency).ToString());
                }
                
            }

            return currencyData;
        }

        private IDictionary<string, string>[] CurrencyToDictionary(Currency[] pCurrencies) {

            return pCurrencies.Select(item => {

                return this.CurrencyToDictionary(item);

            }).ToArray();

        }

        
    }
}