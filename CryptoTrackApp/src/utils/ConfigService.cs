using CryptoTrackApp.src.utils;
using Newtonsoft.Json.Linq;
using System.IO;

namespace CryptoTrackApp.src.utils
{
    public class ConfigService
    {
        private readonly JObject _config;
        private Logger _logger = new Logger();
        public ConfigService(string[] configFilePathArr)
        {
            var path = PathFinder.GetPath(configFilePathArr);
            if (!File.Exists(path))
            {
                _logger.Log("[ERROR - While creating instance of ConfigService - config file not exist!!]");
                throw new FileNotFoundException("Config file not found.", path);
            }

            string json = File.ReadAllText(path);
            _config = JObject.Parse(json);
        }

        public string? GetString(string key)
        {
            return _config[key]?.ToString();
        }

        public int? GetInt(string key)
        {
            return _config[key]?.Value<int>();
        }

        public float? GetFloat(string key)
        {
            return _config[key]?.Value<float>();
        }
    }
}
