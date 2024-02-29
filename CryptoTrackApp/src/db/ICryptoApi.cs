using System.Threading.Tasks;
using CryptoTrackApp.src.models;


namespace CryptoTrackApp.src.db
{
    public interface ICryptoApi
    {
        public Task<Currency> GetCurrency(string pId);
        public Task<Currency[]> GetCurrencies(string[] pIds);
        public Task<Currency[]> GetCurrencies(int offset = 0, int limit = 100);
        
    }
}