using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;
using CryptoTrackApp.src.db;
using CryptoTrackApp.src.models;
using ScottPlot;
using ScottPlot.AxisPanels;

// using GLib;
using ScottPlot.Plottables;
using ScottPlot.TickGenerators;
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
                // return historyValues.TakeLast(182).ToList();
                return historyValues.ToList();

            }
            catch (Exception error)
            {

                Console.WriteLine($"CurrencyService-Error: {error.Message}");
                throw new Exception(error.Message);
            }   
        }


        public async Task<Dictionary<DateTime, List<double>>> GetHistoryValues (string pCurrencyId){

            this._historyValues = await GetHistory(pCurrencyId);
            DateTime actualDate = DateTime.UtcNow;

            Dictionary<DateTime, List<double>> valuesPerMonth = new();


            for (var mes = 6; mes > 0; mes--) {
                valuesPerMonth.Add(actualDate.AddMonths(-mes).AddDays(1-actualDate.Day), new List<double>());
                
                valuesPerMonth.Add(actualDate.AddMonths(-mes).AddDays(1-actualDate.Day).AddDays(14), new List<double>()); // TESTING
            }
            this._historyValues = this._historyValues.Where((elem) => {return valuesPerMonth.Keys.First() <= elem.Item1;}).ToList();
            // Gets the values for each month
            foreach (var value in this._historyValues) {
                var keyToCheck = new DateTime(year: value.Item1.Year, month: value.Item1.Month, day: value.Item1.Day).TimeOfDay;

                foreach (DateTime keyDate in valuesPerMonth.Keys) {

                    if (keyDate.Month == value.Item1.Month && keyDate.Year == value.Item1.Year && 
                    keyDate <= value.Item1 && keyDate.AddDays(14) > value.Item1) {
                        valuesPerMonth[keyDate].Add(Math.Round(value.Item2, 2));
                    }
                }
            }

            return valuesPerMonth;
        }

    }
}