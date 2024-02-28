using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CryptoTrackApp.Migrations;
using CryptoTrackApp.src.models;


namespace CryptoTrackApp.src.services
{
    public interface ICurrencyServices
    {
        public Task<IDictionary<string, object>?> GetCurrency(string pCurrencyId);
        public Task<IDictionary<string, object>[]?> GetCurrencies(int offset=0, int limit=100);
        public Task<IDictionary<string, object>[]?> GetCurrencies(string[] ids);
        public IDictionary<int,double> GetHistory(string pCurrencyId, DateTime pFrom, DateTime pTo);

    }
}