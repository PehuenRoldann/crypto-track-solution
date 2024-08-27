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
        [UI] private Button _loadMoreBtn;

        private List<string> Following {get; set;}
        private ICurrencyServices _curService;
        private ISubscriptionServices _subService;

        private int _offset = 0;
        private int _limit = 50;
        
        public FollowView (ISubscriptionServices pSubService, ICurrencyServices pCurService) 
        : base("FollowView")
        {
            this._curService = pCurService;
            this._subService = pSubService;

            this.CSS_PATH_DARK = "./src/css/follow_view.css";
	        this.CSS_PATH_LIGHT = "";
            this.SetStyle("dark");
            this._mainContainer!.StyleContext.AddClass("main-container");
            this._flowBox!.StyleContext.AddClass("flow-box");
            this._loadMoreBtn!.Hide();
            this._loadMoreBtn.ButtonReleaseEvent += OnLoadMoreBtnRelease;
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
            this._loadMoreBtn.Hide();
            this.Following = await this._subService.GetFollowedCryptosIdsAsync(ViewManager.GetInstance().UserId);
            IDictionary<string, string> [] cryptos = await this._curService.GetCurrencies(offset: this._offset,
                                                                                         limit: this._limit);
            foreach (IDictionary<string, string> crypto in cryptos)
            {
                var cryptoCard = new CryptoCard(this._subService, crypto["Symbol"].ToLower(),
                    crypto["Name"], crypto["Id"], this.Following.Contains(crypto["Id"]));
                cryptoCard.StyleContext.AddClass("crypto-card");
                this._flowBox.Add(cryptoCard);
            }
            //this._loadMoreBtn = new Gtk.Button();
            //this._loadMoreBtn.Label = "Load More Currencies";
            // this._loadMoreBtn.Visible = true;
            //this._flowBox.Add(this._loadMoreBtn);
            this._loadMoreBtn.Show();
        }

        private void OnLoadMoreBtnRelease (object sender, ButtonReleaseEventArgs args)
        {
            Console.WriteLine("Hola!");
            this._offset = this._limit;
            this._limit+= 50;
            this.LoadFlowBox();
        }
    }
}