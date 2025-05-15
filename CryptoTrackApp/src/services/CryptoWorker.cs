using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using CryptoTrackApp.src.models;
using CryptoTrackApp.src.services;
using CryptoTrackApp.src.utils;

public class CryptoWorker
{
    private readonly Timer _timer;
    private readonly ISubscriptionServices _subsServices;
    private readonly ICurrencyServices _currencyServices;
    private readonly string _userId;
    private IDictionary<string, string>[]? _currencies = null;
    private List<string> _followIds;
    private List<IDictionary<string, string>> _subs;
    private Logger _logger = new Logger();
    private IDictionary<string, float> _totalChange = new Dictionary<string, float>();
    private IDictionary<string, float> currencyIdsValues = new Dictionary<string, float>();

    public event EventHandler<CryptoDataEventArgs> NewCryptoDataReceived;
    

    public CryptoWorker(ICurrencyServices currencyServices, ISubscriptionServices subscriptionServices, string userId)
    {
        _currencyServices = currencyServices;
        _subsServices = subscriptionServices;
        _userId = userId;
        _timer = new Timer(50000);  // Intervalo de 60 segundos
        _timer.Elapsed += OnTimerElapsed;
        _timer.AutoReset = true;  // Repetir cada intervalo

    }

    // Iniciar el Worker
    public async void Start()
    {
        await UpdateSubscriptiosn();
        await InitizlizeCurrencyData();
        _timer.Start();
    }

    // Detener el Worker
    public void Stop()
    {
        _timer.Stop();
    }

    private async Task UpdateSubscriptiosn() {
        _followIds = await _subsServices.GetFollowedCryptosIdsAsync(_userId);
        _subs = await _subsServices.GetSubscriptionsAsync(_userId);
    }

    private async Task InitizlizeCurrencyData() {
        _currencies = await _currencyServices.GetCurrencies(_followIds.ToArray());
        foreach (var currency in _currencies) {
            currencyIdsValues.Add(currency[CryptoCurrencyKeys.Id], float.Parse(currency[CryptoCurrencyKeys.ChangePercent24Hr]));
        }
    }

    // Este método se ejecuta cada vez que el Timer se dispara
    private async void OnTimerElapsed(object sender, ElapsedEventArgs e)
    {
        _logger.Log("[EXEC - OnTimerElapsed at CryptoWorker]");
        
        var _newCurrenciesValues = await _currencyServices.GetCurrencies(_followIds.ToArray());
        
        foreach(var newCurrency in _newCurrenciesValues) {

            var notificationThreshold = float.Parse(
                _subs.Where(s => s[SubscriptionKeys.CurrencyId] == newCurrency[CryptoCurrencyKeys.Id])
                .First()[SubscriptionKeys.NotificationThreshold]
                );


            if (!currencyIdsValues.TryGetValue(newCurrency[CryptoCurrencyKeys.Id], out float oldValue))
            {
                oldValue = float.Parse(newCurrency[CryptoCurrencyKeys.ChangePercent24Hr]); 
            }

            var newValue = float.Parse(newCurrency[CryptoCurrencyKeys.ChangePercent24Hr]);
            var change = Math.Abs((newValue - oldValue) / oldValue);

            if (_totalChange.ContainsKey(newCurrency[CryptoCurrencyKeys.Id])) {
                _totalChange[newCurrency[CryptoCurrencyKeys.Id]] = _totalChange[newCurrency[CryptoCurrencyKeys.Id]] + change;
            }
            else {
                _totalChange.Add(newCurrency[CryptoCurrencyKeys.Id], change);
            }

            if (_totalChange[newCurrency[CryptoCurrencyKeys.Id]] >= notificationThreshold && notificationThreshold > 0) {
                _logger.Log($"[RESULT - OnTimerElapsed at CryptoWorker - Threshold surpased for {newCurrency[CryptoCurrencyKeys.Name]}]");
                Console.WriteLine($"Notificación para {newCurrency[CryptoCurrencyKeys.Name]}");
                _totalChange[newCurrency[CryptoCurrencyKeys.Id]] = 0;
                currencyIdsValues[newCurrency[CryptoCurrencyKeys.Id]] = float.Parse(newCurrency[CryptoCurrencyKeys.ChangePercent24Hr]);
            }
            
        }
        
        // Disparar el evento con los nuevos datos
        OnNewCryptoDataReceived(new CryptoDataEventArgs { CryptoData = _newCurrenciesValues });
    }

    // Método para disparar el evento
    protected virtual void OnNewCryptoDataReceived(CryptoDataEventArgs e)
    {
        GLib.Idle.Add(() =>
        {
            NewCryptoDataReceived?.Invoke(this, e);
            return false; // Devuelve false para que no se ejecute de nuevo
        });
    }
}

public class CryptoDataEventArgs : EventArgs
{
    public IDictionary<string, string>[] CryptoData { get; set; }
}

