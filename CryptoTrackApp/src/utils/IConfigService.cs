namespace CryptoTrackApp.src.utils
{
    public interface IConfigService
    {
        string? GetString(string key);
        int? GetInt(string key);
        float? GetFloat(string key);
        void SetKey(string key, string value);
    }
}
