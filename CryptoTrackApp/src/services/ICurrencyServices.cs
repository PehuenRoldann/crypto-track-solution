using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;



namespace CryptoTrackApp.src.services
{
    public interface ICurrencyServices
    {
        public Task<IDictionary<string, string>> GetCurrency(string pCurrencyId);
        public Task<IDictionary<string, string>[]> GetCurrencies(int offset=0, int limit=100);
        public Task<IDictionary<string, string>[]> GetCurrencies (string[] pIds);
        //public Task<IDictionary<TimestampAttribute,double> > GetHistory(string pCurrencyId);

    }
}