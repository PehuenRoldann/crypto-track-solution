using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CryptoTrackApp.Migrations;
using CryptoTrackApp.src.models;


namespace CryptoTrackApp.src.services
{
    public interface ICurrencyServices
    {
        public Task<Currency?> GetCurrency(string pCurrencyId);
        public IDictionary<int,double> GetHistory(string pCurrencyId, DateTime pFrom, DateTime pTo);

    }
}