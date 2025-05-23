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

        // Radio Buttions Items:
        [UI] private RadioMenuItem _rankingItem;
        [UI] private RadioMenuItem _highPriceItem;
        [UI] private RadioMenuItem _lowPriceItem;
        [UI] private RadioMenuItem _growingItem;
        [UI] private RadioMenuItem _loweringItem;

        private List<string> _following { get; set; }
        private ICurrencyServices _curService;
        private ISubscriptionServices _subService;

        private int _offset = 0;
        private int _limit = 50;
        private List<CryptoCard> _cardsArray = new List<CryptoCard>();
        private Logger _logger = new Logger();
        public FollowView (ISubscriptionServices pSubService, ICurrencyServices pCurService) 
        : base("FollowView")
        {
            this._curService = pCurService;
            this._subService = pSubService;


            this.SetStyle(CssFilesPaths.FollowViewCss);
            
            this._loadMoreBtn!.Hide();

            this._mainContainer!.StyleContext.AddClass("main-container");

            this._flowBox!.StyleContext.AddClass("flow-box");
            
            
            
            this._spinner!.Active = false;

            this.LoadFlowBox();
            this.ShowAll(); 
            
        }

        public override void ConfigEventHandlers()
        {

            _loadMoreBtn.ButtonReleaseEvent += OnLoadMoreBtnRelease;
            _searchEntry!.Changed += OnSearchEntryChange!;

            // Radio buttons events
            _rankingItem.Toggled += OnSortOptionToggled;
            _highPriceItem.Toggled += OnSortOptionToggled;
            _lowPriceItem.Toggled += OnSortOptionToggled;
            _growingItem.Toggled += OnSortOptionToggled;
            _loweringItem.Toggled += OnSortOptionToggled;

        }

        private void OnSortOptionToggled(object? sender, EventArgs e)
        {
            if (sender is RadioMenuItem item && item.Active)
            {
                string selected = item.Label; // o item.Name si usás identificadores únicos

                List<CryptoCard> sortedCards = selected switch
                {
                    "High Price" => _cardsArray.OrderByDescending(c => c.PriceUsd).ToList(),
                    "Low Price" => _cardsArray.OrderBy(c => c.PriceUsd).ToList(),
                    "Growing Tendency" => _cardsArray.OrderByDescending(c => c.ChangePercent24Hr).ToList(),
                    "Lowering Tendency" => _cardsArray.OrderBy(c => c.ChangePercent24Hr).ToList(),
                    "Ranking" or _ => _cardsArray.OrderBy(c => c.Rank).ToList(),
                };

                ReloadFlowBox(sortedCards.ToArray());
            }
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

            try
            {
                IDictionary<string, string>[] cryptos = await this._curService.GetCurrencies(offset: this._offset, limit: this._limit);
                foreach (IDictionary<string, string> crypto in cryptos)
                {
                    var cryptoCard = new CryptoCard(
                        this._subService,
                        crypto[CryptoCurrencyKeys.Symbol].ToLower(),
                        crypto[CryptoCurrencyKeys.Name],
                        crypto[CryptoCurrencyKeys.Id],
                        decimal.Parse(crypto[CryptoCurrencyKeys.PriceUsd]),
                        float.Parse(crypto[CryptoCurrencyKeys.ChangePercent24Hr]),
                        int.Parse(crypto[CryptoCurrencyKeys.Rank]),
                        this._following.Contains(crypto[CryptoCurrencyKeys.Id])
                    );

                    cryptoCard.StyleContext.AddClass("crypto-card");

                    if (!cryptoCard.AlreadyFollow)
                    {
                        this._cardsArray.Add(cryptoCard);
                        this._flowBox.Add(cryptoCard);
                    }

                }
                this._flowBox.ShowAll();

            }
            catch (Exception error)
            {
                this._logger.Log($"[ERROR - LoadFlowBox at FollowView - message:{error.Message}]");
            }
            finally
            {
                this._spinner.Active = false;
                this._spinner_container.Visible = false;

                this._loadMoreBtn.Show();
            }
        }

        private async void OnLoadMoreBtnRelease (object sender, ButtonReleaseEventArgs args)
        {

            this._offset = this._limit;
            this._limit+= 50;

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
                    var cryptoCard = new CryptoCard(
                        this._subService,
                        crypto[CryptoCurrencyKeys.Symbol].ToLower(),
                        crypto[CryptoCurrencyKeys.Name],
                        crypto[CryptoCurrencyKeys.Id],
                        decimal.Parse(crypto[CryptoCurrencyKeys.PriceUsd]),
                        float.Parse(crypto[CryptoCurrencyKeys.ChangePercent24Hr]),
                        int.Parse(crypto[CryptoCurrencyKeys.Rank]),
                        this._following.Contains(crypto[CryptoCurrencyKeys.Id])
                    );
                    cryptoCard.StyleContext.AddClass("crypto-card");
                    this._cardsArray.Add(cryptoCard);
                    this.ReloadFlowBox(this._cardsArray.ToArray());
                }


            } catch( Exception error) {
                this._logger.Log($"[ERROR - OnLoadMoreBtnRelease at FollowView - message: {error.Message}]");
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