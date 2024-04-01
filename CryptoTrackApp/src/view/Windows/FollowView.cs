using System;
using System.Collections.Generic;
using CryptoTrackApp.src.services;
using CryptoTrackApp.src.view.Components;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace CryptoTrackApp.src.view.Windows
{
    public class FollowView : View
    {
        [UI] private Box _mainContainer;
        [UI] private Box _searchContainer;
        [UI] private FlowBox _flowBox;

        private ICurrencyServices currencyServices = new CurrencyServices();
        
        public FollowView () : base("FollowView")
        {

            this.CSS_PATH_DARK = "./src/css/follow_view.css";
	        this.CSS_PATH_LIGHT = "";
            this.SetStyle("dark");
            this._mainContainer.StyleContext.AddClass("main-container");
            this._flowBox.StyleContext.AddClass("flow-box");
            
            this.LoadFlowBox();
            this.ShowAll(); 
            
        }

        public override void ConfigEventHandlers()
        {
            // throw new NotImplementedException();
        }

        public override void ConfigImages()
        {
            // throw new NotImplementedException();
        }


        private async void LoadFlowBox()
        {
            // var cryptos = new List<string>() {"dai", "btc", "cob", "cnd", "dat", "doge", "dth", "eth"};

            IDictionary<string, string> [] cryptos = await currencyServices.GetCurrencies(offset: 0, limit: 50);

            /* foreach (string crypto in cryptos)
            {
                var cryptoCard = new CryptoCard(crypto);
                cryptoCard.StyleContext.AddClass("crypto-card");
                this._flowBox.Add(cryptoCard);
            } */

            foreach (IDictionary<string, string> crypto in cryptos)
            {
                var cryptoCard = new CryptoCard(crypto["Symbol"].ToLower(), crypto["Name"], crypto["Id"]);
                cryptoCard.StyleContext.AddClass("crypto-card");
                this._flowBox.Add(cryptoCard);
            }

        }
    }
}