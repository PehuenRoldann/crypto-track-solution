using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CryptoTrackApp.src.models;


namespace CryptoTrackApp.src.db
{
    public interface ICryptoApi
    {

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
        public Task<Currency> GetCurrency(string pId);

        /// <summary>
        /// Gets the selected currency from the Coin Api service.
        /// </summary>
        /// <param name="pIds">Currency's Id we want to query.</param>
        /// <returns>Currency</returns>
        /// <exception cref="Exception">
        /// Throws an exception when the Http status of the response is different from Ok (200).
        /// </exception>
        public Task<Currency[]> GetCurrencies(string[] pIds);

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
        /// </exception>
        public Task<Currency[]> GetCurrencies(int offset = 0, int limit = 100);

        /// <summary>
        /// Gets the list of the history values for a given currency
        /// </summary>
        /// <param name="pId">Currency id</param>
        /// <returns>List with pairs of date times and values</returns>
        public Task<List<(DateTime, double)>> GetHistory(string pId);
    }
}