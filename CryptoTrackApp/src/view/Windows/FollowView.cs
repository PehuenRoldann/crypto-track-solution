using System;
using System.Collections.Generic;
using CryptoTrackApp.src.services;
using CryptoTrackApp.src.view.Components;
using CryptoTrackApp.src.view.Controllers;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace CryptoTrackApp.src.view.Windows
{
    public class FollowView : View
    {
        [UI] private Box _mainContainer;
        [UI] private Box _searchContainer;
        [UI] private FlowBox _flowBox;

        private List<string> Following {get; set;}
        private ICurrencyServices _curService;
        private ISubscriptionServices _subService;
        
        public FollowView (ISubscriptionServices pSubService, ICurrencyServices pCurService) 
        : base("FollowView")
        {
            this._curService = pCurService;
            this._subService = pSubService;

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
            this.Following = await this._subService.GetFollowedCryptosIdsAsync(ViewManager.GetInstance().UserId);
            IDictionary<string, string> [] cryptos = await this._curService.GetCurrencies(offset: 0,
                                                                                         limit: 50);
            foreach (IDictionary<string, string> crypto in cryptos)
            {
                var cryptoCard = new CryptoCard(this._subService, crypto["Symbol"].ToLower(),
                    crypto["Name"], crypto["Id"], this.Following.Contains(crypto["Id"]));
                cryptoCard.StyleContext.AddClass("crypto-card");
                this._flowBox.Add(cryptoCard);
            }

        }
    }
}