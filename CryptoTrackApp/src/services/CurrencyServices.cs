using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;
using CryptoTrackApp.src.db;
using CryptoTrackApp.src.models;
using ScottPlot.TickGenerators.TimeUnits;
using SP = ScottPlot;

namespace CryptoTrackApp.src.services
{

    public class CurrencyServices : ICurrencyServices
    {
        private ICryptoApi cryptoApi;
        private List <(DateTime, double)> _historyValues;

        public CurrencyServices()
        {

            this.cryptoApi = new CoinApi();
        }

//----------------------- AUXILIAR METHODS ----------------------------------------------------------

        private IDictionary<string, string> CurrencyToDictionary (Currency pCurrency)
        {
            
            IDictionary<string, string> currencyData = new Dictionary<string, string>();

            foreach (var property in pCurrency.GetType().GetProperties()) {

                if (property.GetValue(pCurrency) == null)
                {
                    currencyData.Add(property.Name, "");
                }
                else
                {
                    currencyData.Add(property.Name, property.GetValue(pCurrency).ToString());
                }
                
            }

            return currencyData;
        }

        private IDictionary<string, string>[] CurrencyToDictionary(Currency[] pCurrencies)
        {

            return pCurrencies.Select(item => {

                return this.CurrencyToDictionary(item);

            }).ToArray();

        }

// ------------------- INTERFACE IMPLEMENTATIONS -------------------------------------------------

        /// <summary>
        /// Returns a currency with the given Id.
        /// </summary>
        /// <param name="pCurrencyId">Id of the currency we want to return</param>
        /// <returns>
        /// IDictionary<string, string> with the currency inforreturn "";mation.
        /// </returns>
        public async Task<IDictionary<string, string>> GetCurrency(string pCurrencyId)
        {

            try {
                Currency currency = await this.cryptoApi.GetCurrency(pCurrencyId);
                return this.CurrencyToDictionary(currency);
            }
            catch (Exception error) {

                Console.WriteLine($"CurrencyService-Error: {error.Message}");
                throw error;
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

        public async Task<IDictionary<string, string>[]> GetCurrencies(string[] pIds)
        {
            try {

                Currency[] currencyData = await this.cryptoApi.GetCurrencies(pIds);
                return this.CurrencyToDictionary(currencyData);
            }
            catch (Exception error) {
                Console.WriteLine($"CurrencyService-Error: {error.Message}");
                return new Dictionary<string, string>[0];
            }
        }

        public async Task<List<(DateTime, double)>> GetHistory(string pCurrencyId)
        {
            try
            {
                List<(DateTime, double)> historyValues = await this.cryptoApi.GetHistory(pCurrencyId);

                // 182 days aprox. to 6 months:
                return historyValues.TakeLast(182).ToList();

            }
            catch (Exception error)
            {

                Console.WriteLine($"CurrencyService-Error: {error.Message}");
                throw new Exception(error.Message);
            }

            
        }


        /// <summary>
        /// Generates a image with the plotbox.
        /// </summary>
        /// <param name="pCurrencyId"></param>
        /// <returns>Path to the plotbox generated image</returns>
        public async Task<string> GetBoxPlot (string pCurrencyId) 
        {
            this._historyValues = await GetHistory(pCurrencyId);
            DateTime actualDate = DateTime.UtcNow;

            Dictionary<DateTime, List<double>> valuesPerMonth = new Dictionary<DateTime, List<double>>();

            
            for (var mes = 6; mes > 0; mes--) {
                valuesPerMonth.Add(actualDate.AddMonths(-mes).AddDays(1-actualDate.Day), new List<double>());
            }

            foreach (var value in this._historyValues) {
                var keyToCheck = new DateTime(year: value.Item1.Year, month: value.Item1.Month, 1).TimeOfDay;

                foreach (DateTime keyDate in valuesPerMonth.Keys) {

                    if (keyDate.Month == value.Item1.Month && keyDate.Year == value.Item1.Year) {
                        valuesPerMonth[keyDate].Add(value.Item2);
                    }
                }
            }
        
            return "";
                        
        }
    }
}