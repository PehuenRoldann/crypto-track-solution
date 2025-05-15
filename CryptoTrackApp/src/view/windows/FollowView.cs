using System;
using System.Collections.Generic;
using System.Linq;
using CryptoTrackApp.src.services;
using CryptoTrackApp.src.utils;
using CryptoTrackApp.src.view.components;
using CryptoTrackApp.src.view.helpers;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace CryptoTrackApp.src.view.windows
{
    public class FollowView : View
    {
        [UI] private Box _mainContainer;
        [UI] private SearchEntry _searchEntry;
        [UI] private FlowBox _flowBox;
        [UI] private Button _loadMoreBtn;
        [UI] private Spinner _spinner;
        [UI] private AspectFrame _spinner_container;

        private List<string> _following {get; set;}
        private ICurrencyServices _curService;
        private ISubscriptionServices _subService;

        private int _offset = 0;
        private int _limit = 50;
        private List<CryptoCard> _cardsArray = new List<CryptoCard>();
        
        public FollowView (ISubscriptionServices pSubService, ICurrencyServices pCurService) 
        : base("FollowView")
        {
            this._curService = pCurService;
            this._subService = pSubService;


            /* this.CSS_PATH_DARK = "./src/css/follow_view.css";
	        this.CSS_PATH_LIGHT = ""; */
            this.SetStyle(CssFilesPaths.FollowViewCss);
            
            this._loadMoreBtn!.Hide();
            this._loadMoreBtn.ButtonReleaseEvent += OnLoadMoreBtnRelease;

            this._mainContainer!.StyleContext.AddClass("main-container");

            this._flowBox!.StyleContext.AddClass("flow-box");
                        // Configurar el GtkFlowBox
            // this._flowBox.Homogeneous = false; // Asegúrate de que no sea homogéneo

            // Opcional: configurar el ajuste de los hijos si es necesario
            /* this._flowBox.SetColumnSpacing(10); // Espacio entre columnas
            this._flowBox.SetRowSpacing(10);    // Espacio entre filas */

            this._searchEntry!.Changed += OnSearchEntryChange!;
            
            this._spinner!.Active = false;

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
            this._spinner_container.Visible = true;
            this._spinner.Active = true;

            if (this._following == null) {
                this._following = await this._subService.GetFollowedCryptosIdsAsync(ViewManager.GetInstance().UserId);
            }

            try{
                IDictionary<string, string> [] cryptos = await this._curService.GetCurrencies(offset: this._offset,limit: this._limit);
                foreach (IDictionary<string, string> crypto in cryptos)
                {
                    var cryptoCard = new CryptoCard(this._subService, crypto["Symbol"].ToLower(),
                        crypto["Name"], crypto["Id"], this._following.Contains(crypto["Id"]));
                    cryptoCard.StyleContext.AddClass("crypto-card");
                    this._cardsArray.Add(cryptoCard);
                    Console.WriteLine(this._cardsArray.Count()); // DEBUG */
                    this._flowBox.Add(cryptoCard);
                    this._flowBox.ShowAll();
                }


            } catch( Exception error) {
                Console.WriteLine(error.Message);
            } finally {
                this._spinner.Active = false;
                this._spinner_container.Visible = false;

                this._loadMoreBtn.Show();
            }
        }

        private async void OnLoadMoreBtnRelease (object sender, ButtonReleaseEventArgs args)
        {

            this._offset = this._limit;
            this._limit+= 50;
            // this.LoadFlowBox();

            this._loadMoreBtn.Hide();
            this._spinner_container.Visible = true;
            this._spinner.Active = true;

            if (this._following == null) {
                this._following = await this._subService.GetFollowedCryptosIdsAsync(ViewManager.GetInstance().UserId);
            }

            try{
                IDictionary<string, string> [] cryptos = await this._curService.GetCurrencies(offset: this._offset,limit: this._limit);
                foreach (IDictionary<string, string> crypto in cryptos)
                {
                    var cryptoCard = new CryptoCard(this._subService, crypto["Symbol"].ToLower(),
                        crypto["Name"], crypto["Id"], this._following.Contains(crypto["Id"]));
                    cryptoCard.StyleContext.AddClass("crypto-card");
                    this._cardsArray.Add(cryptoCard);
                    this.ReloadFlowBox(this._cardsArray.ToArray());
                }


            } catch( Exception error) {
                Console.WriteLine(error.Message);
            } finally {
                this._spinner.Active = false;
                this._spinner_container.Visible = false;
                this._loadMoreBtn.Show();
            }
        }

        private void ReloadFlowBox(CryptoCard[] cards)
        {
            // Remover todos los widgets del FlowBox
            foreach (var child in this._flowBox.Children)
            {
                this._flowBox.Remove(child);
            }

            foreach( var card in cards) {
                /* Console.WriteLine("Card");
                Console.Write(card); */

                // Verifica si la tarjeta ya tiene un padre
                if (card.Parent != null)
                {
                    ((Container)card.Parent).Remove(card);
                }

                this._flowBox.Add(card);
            }

            this._flowBox.ShowAll();

        }

        private void OnSearchEntryChange (object? sender, EventArgs? args)
        {
            string searchText = this._searchEntry.Text.ToLower();
            Console.WriteLine(searchText); // DEBUG
            if (searchText != ""){
                CryptoCard[] filteredArray = this._cardsArray.Where(card => card.CryptoName.ToLower().Contains(searchText)).ToArray();
                this.ReloadFlowBox(filteredArray!);
            }
            else {
                this.ReloadFlowBox(this._cardsArray.ToArray());
            }
        }

        
    }
}