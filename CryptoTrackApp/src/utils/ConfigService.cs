using Newtonsoft.Json.Linq;
using System.IO;
using System;

namespace CryptoTrackApp.src.utils
{
    public class JsonConfigService : IConfigService
    {
        private readonly JObject _config;
        private Logger _logger = new Logger();

        private static JsonConfigService _instance;
        private static readonly object _lock = new Object();

        // ------ SINGLETON CONSTRUCTOR -----------------------
        private JsonConfigService()
        {
            var path = PathFinder.GetPath(Config.JsonConfArrPath);
            if (!File.Exists(path))
            {
                _logger.Log("[ERROR - While creating instance of ConfigService - config file not exist!!]");
                throw new FileNotFoundException("Config file not found.", path);
            }

            string json = File.ReadAllText(path);
            _config = JObject.Parse(json);
        }

        public static JsonConfigService GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new JsonConfigService();
                    }
                }
            }

            return _instance;
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
