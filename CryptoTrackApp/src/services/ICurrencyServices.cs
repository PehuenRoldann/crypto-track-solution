using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;



namespace CryptoTrackApp.src.services
{
    public interface ICurrencyServices
    {
        public Task<IDictionary<string, string>> GetCurrency(string pCurrencyId);

        /// <summary>
        /// Returs a collection with the information of crypto currencies.
        /// </summary>
        /// <param name="pOffset">The number of currencies we have already queried before.</param>
        /// <param name="pLimit">The number of currencies we want to receive.</param>
        /// <returns>
        /// A collection if Dictionary<string, string> with the currency information as an Array.
        /// Each currency has the next fields: Id, Rank, Symbol, Name, Supply, MaxSupply, MarketCpaUsd,
        ///   VolumeUsd24Hr, PriceUsd, ChangePercent24Hr, VWap24Hr, Explorer.
        /// </returns>
        /// <exception cref="Exception">
        /// If the query fails for some reason.
        /// </exception>
        public Task<IDictionary<string, string>[]> GetCurrencies(int offset=0, int limit=100);
        public Task<IDictionary<string, string>[]> GetCurrencies (string[] pIds);
        public Task<List<(DateTime, double)>> GetHistory(string pCurrencyId);

    }
}