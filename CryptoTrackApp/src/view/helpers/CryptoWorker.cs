using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using TM = System.Timers.Timer;
using CryptoTrackApp.src.services;
using CryptoTrackApp.src.utils;


namespace CryptoTrackApp.src.view.helpers 
{

    public class CryptoWorker
    {
        private readonly TM _timer;
        private readonly ISubscriptionServices _subsServices;
        private readonly ICurrencyServices _currencyServices;
        private readonly string _userId;
        private Logger _logger = new Logger();
        private ICryptoNotifier  _cryptoNotifier = new DesktopCryptoNotifier();

        public event EventHandler<CryptoDataEventArgs> NewCryptoDataReceived;
        

        public CryptoWorker(
            ICurrencyServices currencyServices,
            ISubscriptionServices subscriptionServices,
            string userId
        )
        {
            IConfigService configService = JsonConfigService.GetInstance();
            _currencyServices = currencyServices;
            _subsServices = subscriptionServices;
            _userId = userId;
            int timeoutValue = configService.GetInt(ConfigurationsKeys.MainViewTimeoutRefresh) ?? 10; // default cada 10 minutos
            int milisecondsTimeout = 1000 * 60 * timeoutValue;
            _timer = new TM(milisecondsTimeout);
            _timer.Elapsed += OnTimerElapsed;
            _timer.AutoReset = true;  // Repetir cada intervalo

        }

        // Iniciar el Worker
        public async void Start()
        {
            _timer.Start();
        }

        // Detener el Worker
        public void Stop()
        {
            this._timer.Stop();
        }

        // Este método se ejecuta cada vez que el Timer se dispara
        private async void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _logger.Log("[EXEC - OnTimerElapsed at CryptoWorker - fetching follow Ids]");
            var followIds = (await this._subsServices.GetFollowedCryptosIdsAsync(_userId)).ToArray();

            if (followIds.Length == 0) {
                _logger.Log("[INFO - CryptoWorker - There are no subscriptions to search for changes.]");
            }

            var newCurrenciesValues = followIds.Length > 0 
            ? await _currencyServices.GetCurrencies(followIds)
            : null;

            if (newCurrenciesValues != null && newCurrenciesValues.Length > 0){
                _logger.Log("[RESULT - OnTimerElapsed at CryptoWorker - found new data to refresh]");
                this.NotifyTendencyChanges(newCurrenciesValues);
                OnNewCryptoDataReceived(new CryptoDataEventArgs { CryptoData = newCurrenciesValues });
            }    
        }


        /// <summary>
        /// Compares the percentage change values ​​over 24 hours and notifies if the threshold is exceeded.
        /// </summary>
        /// <param name="pNewCurrenciesData">Currency data to compare</param>
        private async void NotifyTendencyChanges(IDictionary<string, string>[] pNewCurrenciesData) {

            List<IDictionary<string, string>> subsData = await this._subsServices.GetSubscriptionsAsync(_userId) ?? new();

            if (subsData.Count == 0) {
                return;
            } 

            List<string> changesToNotfy = new();

            foreach (var currencyData in pNewCurrenciesData) {
                string currencyId = currencyData[CryptoCurrencyKeys.Id];
                IDictionary<string, string>? subscription = subsData.FirstOrDefault(x => x[SubscriptionKeys.CurrencyId] == currencyId);

                if (subscription == null) continue;

                var notificationThreshold = float.Parse(subscription[SubscriptionKeys.NotificationThreshold]);

                var changePercent24Hr = float.Parse(currencyData[CryptoCurrencyKeys.ChangePercent24Hr]);

                var absoluteChange = Math.Round(Math.Abs(changePercent24Hr), 2);
                
                if (notificationThreshold > 0 && absoluteChange >= notificationThreshold) {

                    this._logger.Log($"[EXEC - Operation NotifyTendencyChanges - userId: {_userId}; currencyId: {currencyId}]");

                    string currencyName = currencyData[CryptoCurrencyKeys.Name];
                    string changeType = changePercent24Hr < 0 ? "disminuido" : "aumentado";
                    changesToNotfy.Add($"El valor de {currencyName} ha {changeType} un {absoluteChange}%.");
                }

            }


            if (changesToNotfy.Count > 0) {

                string message = "Cambios importantes";
                foreach (var change in changesToNotfy) {
                    message += "\n" + change + "";
                }

                this._cryptoNotifier.Notify(message);

            }

            

        }


        // Método para disparar el evento y actualizar los valores en la tabla de la vista.
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

}