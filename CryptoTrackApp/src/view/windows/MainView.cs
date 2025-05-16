using System;
using System.Collections.Generic;
using Gtk;
using Gdk;
using UI = Gtk.Builder.ObjectAttribute;
using CryptoTrackApp.src.view.components;
using CryptoTrackApp.src.services;
using System.Threading.Tasks;
using Pango;
using System.Linq;
using CryptoTrackApp.src.utils;
using CryptoTrackApp.src.view.helpers;

namespace CryptoTrackApp.src.view.windows
{
    
    public class MainView : View
    {
        [UI] private Button _panelBtn;
        [UI] private Button _followButton;
        [UI] private Button _logOutBtn;
        [UI] private Button _aboutBtn;
        [UI] private Image _logoImg;
        [UI] private Image _logoutBtnImg;
        [UI] private Box _panel;
        [UI] private Box _panelMessage;
        [UI] private Box _middlePanel;
        [UI] private Box _panelScroll;
        [UI] private Box _panelBoxPlot;
        [UI] private Image _panelBtnImg;
        [UI] private Image _followBtnImg;
        [UI] private Image _aboutBtnImg;
        [UI] private AspectFrame _panelTable;
        [UI] private Spinner _spinner;

        private string _userId;
        private CryptoTreeViewComponent? _subsTree;
        private ISubscriptionServices _subscriptionService;
        private ICurrencyServices _currencyService;
        private IPlotterService _plotService;

        private IDictionary<string,string>[] _currenciesData;
        private List<IDictionary<string, string>> _activeSubscriptions;
        private string _selectedCryptoId = "";

        private Logger _logger = new Logger();


        private const int PLOT_WIDTH = 900;
        private const int PLOT_HEIGHT = 400;

        private string[] _LOGOUT_IMAGE_PATH = {"src", "assets", "images", "logout.png"};
        private string[] _LOGO_PATH = {"src", "assets", "images", "cta_logo_64x64.png"};
        private string[] _NOT_FOUND_PATH = {"src", "assets", "images", "not_found.png"};
        private string[] _PANEL_IMG_PATH = {"src", "assets", "images", "panel.png"};
        private string[] _COMPAS_IMG_PATH = {"src", "assets", "images", "compass.png"};
        private string[] _ABOUT_IMG_PATH = {"src", "assets", "images", "about.png"};
        
        private string[] _SERVER_BURNING_PATH = {"src", "assets", "images", "server_burning.png"};
        
        private readonly CryptoWorker _cryptoWorker;
        private IViewManager _viewManager = ViewManager.GetInstance();


        public MainView(
            string pUserId,
            ISubscriptionServices pSubService,
            ICurrencyServices pCurrencyService,
            IPlotterService pPlotService) : base(Templates.MainView)
        {
            _logger.Log("[INIT - MainView]");
            this._userId = pUserId;
            this._subscriptionService = pSubService;
            this._currencyService = pCurrencyService;
            this._plotService = pPlotService;

            this.InitPanel();
            this.SetStyle(CssFilesPaths.MainViewCss);

            _cryptoWorker = new CryptoWorker(this._currencyService, this._subscriptionService, _userId);
            _cryptoWorker.NewCryptoDataReceived += UpdateData;
            _cryptoWorker.Start();
        }

        private async void UpdateData(object? sender, CryptoDataEventArgs e)
        {
            try {
                this._panelScroll.Hide();
                this._panelMessage.Hide();
                this._spinner.Show();
                ResetSubsTree();
            }
            catch(Exception error) {
                _logger.Log($"[ERROR - UpdateData at Main view - Message: {error.Message}]");
            }
                
        }

        // ----------- INITIAL CONFIGURATIONS ---------------------------------------
        private void OnDelete(object o, DeleteEventArgs args)
        {
            Application.Quit();
        }

        public override void ConfigEventHandlers()
        {
            this._followButton.ButtonReleaseEvent += FollowButtonReleased;
            this._panelBtn.ButtonReleaseEvent += PanelButtonReleased;
            this._logOutBtn.ButtonReleaseEvent += LogOutButtonReleased;
            this._aboutBtn.ButtonReleaseEvent += AboutButtonReleased;
        }

        private void AboutButtonReleased(object o, ButtonReleaseEventArgs args)
        {
            var dialog = new AboutConfigDialog(this);
            dialog.Run();
        }

        private void LogOutButtonReleased(object o, ButtonReleaseEventArgs args)
        {
            _cryptoWorker.Stop();
            _viewManager.ShowView(ViewsIds.Login, this);
        }

        public override void ConfigImages()
        {
            this._logoImg.File = this.GetAbsolutePath(_LOGO_PATH);
            this._logoutBtnImg.File = this.GetAbsolutePath(_LOGOUT_IMAGE_PATH);
            this._panelBtnImg.File = this.GetAbsolutePath(_PANEL_IMG_PATH);
            this._followBtnImg.File = this.GetAbsolutePath(_COMPAS_IMG_PATH);
            this._aboutBtnImg.File = this.GetAbsolutePath(_ABOUT_IMG_PATH);
        }
        
        private async void PanelButtonReleased(object sender, ButtonReleaseEventArgs args)
        {
            this._panelScroll.Hide();
            this._panelMessage.Hide();
            this._spinner.Show();
            await ResetSubsTree();
            LoadBoxPlot();

        }

        private async void InitPanel()
        {
            
            this._panel.Halign = Align.Start;
            this._middlePanel.Halign = Align.Center;
            ConfigSpinner();
            this._spinner.Active = true;
            this._middlePanel.Hide();
            this._spinner.Show();
            await LoadTablePanel();
            LoadBoxPlot();
 
        }


        /// <summary>
        /// Loads and show the 
        /// </summary>
        public async Task LoadTablePanel () {
            
            await this.UpdateCurrenciesData();

            if (this._subsTree == null)
            {
                ConfigSubsList();
            }

            int haveSubscriptions = await this.LoadSubscriptionsList();

            switch (haveSubscriptions) {
                case 0:
                    ShowMessagePanel(
                        pMessage: "You are not following any crypto yet!\n"+
                        "Press the follow button in the navbar to start following some currencies.",
                        pImagePath: this.GetAbsolutePath(_NOT_FOUND_PATH)
                    );
                break;
                case 1:
                    ShowMessagePanel(
                        pMessage: "This application uses a third party API to get the currencies info.\n"
                        +"Please, wait a few seconds and try again.", 
                        pImagePath: this.GetAbsolutePath(_SERVER_BURNING_PATH)
                    );
                break;
                case 2:
                    this._panelScroll.Add(this._subsTree);
                    this._spinner.Hide();
                    this._panelMessage.Hide();
                    this._panelScroll.ShowAll();
                    this._middlePanel.ShowAll();
                break;
            }

        }

        public void ConfigSubsList()
        {
            CryptoTreeViewComponent subsTree = new CryptoTreeViewComponent(width: 500, heigh:350);
            subsTree.RowActivatedEvent += (sender, e) =>
            {
                _logger.Log($"[EVENT - RowAcitvatedEvent - Row values: [Name: {e.Name}, Rank: {e.Rank}, UsdPrice: {e.UsdPrice}, Tendency: {e.Tendency}]]");
                IDictionary<string, string> currency = this._currenciesData.First<IDictionary<string, string>>(elem => elem[CryptoCurrencyKeys.Name] == e.Name);
                this.LoadBoxPlot(currency[CryptoCurrencyKeys.Id]);

            };

            subsTree.UnfollowEvent += UnfollowPressedEventHandler;
            subsTree.NotificationEditedEvent += NotificationEditedEventHandler;

            subsTree.Expand = true;
            subsTree.Halign = Align.Center;
            subsTree.Valign = Align.Center;
            if (!subsTree.StyleContext.HasClass(MainViewClases.SubsTree)) {
                subsTree.StyleContext.AddClass(MainViewClases.SubsTree);
            }
            this._subsTree = subsTree;

        }

        private async void NotificationEditedEventHandler(object? sender, NotificationEditedEventArgs e)
        {
            var currencyData = this._currenciesData.Where(c => c[CryptoCurrencyKeys.Id] == e.CurrencyId).FirstOrDefault();
            var infoDialog = new InformationDialog(this, "Updating notification Threshold");
            infoDialog.Show();
            await Task.Delay(2000); // Debug
            var result = await this._subscriptionService.SetNotificationTreshold(_userId, currencyData![CryptoCurrencyKeys.Id], e.UmbralValue);
            await UpdateCurrenciesData();
            await ResetSubsTree();
            if (result) {
                infoDialog.ShowContent("Threshold updated successfully", ImagesArrPaths.CheckMark);
            }
            else {
                infoDialog.ShowContent(
                    "Error while updating threshold...\n" +
                    "Try again or contact support.",
                    ImagesArrPaths.CheckMark);
            }
        }

        public void UnfollowPressedEventHandler (object sender, UnfollowEventArgs e) {

            
            var imgPath = AssetsArrPaths.CurrenciesImages;
            var currencySymbol = this._currenciesData.First(c => c[CryptoCurrencyKeys.Id] == e.CurrencyId)[CryptoCurrencyKeys.Symbol];
            var unfollowDialog = new ConfirmationDialog(
                this,
                "Unfollow",
                $"Are you sure about unfollow {e.Name}?",
                pImgPathArr: imgPath.Append($"{currencySymbol.ToLower()}.svg").ToArray()
            );


            unfollowDialog.ConfirmationBtn.Released += async (s, a) => {

                unfollowDialog.Destroy();
                var infoDialog = new InformationDialog(this, $"Unfollowing {e.Name}");
                infoDialog.Show();
                await Task.Delay(2000);
                bool result = await this._subscriptionService.UnfollowAsync(_userId, e.CurrencyId);
                await ResetSubsTree();
                if (result)
                {
                    infoDialog.ShowContent($"You have stoped following {e.Name}", ImagesArrPaths.CheckMark);
                    LoadBoxPlot();
                }
                else
                {
                    infoDialog.ShowContent($"An unexpected error has occurred", ImagesArrPaths.CrosskMark);
                }
                
            };

            unfollowDialog.ShowAll();

        }


        private async Task UpdateCurrenciesData() {

            _activeSubscriptions = await _subscriptionService.GetActiveSubscriptionsListAsync(this._userId) ?? new List<IDictionary<string, string>>();
            List<string> cryptosId = new List<string>();

            if (_activeSubscriptions.Count > 0)
            {
                cryptosId = _activeSubscriptions.Select(s => s[SubscriptionKeys.CurrencyId]).ToList();
                if (cryptosId.Count > 0)
                {
                    _currenciesData = await _currencyService.GetCurrencies(cryptosId.ToArray());
                }
            }
            else
            {
                _currenciesData = new IDictionary<string, string>[0];
            }

            

        }
        /// <summary>
        /// Load the subscriptions List for the first time
        /// </summary>
        /// <returns>
        /// 0: No subscriptions.
        /// 1: There are subscriptions, but there was a problem getting the info.
        /// 2: The information has ben retrived successfully.
        /// </returns>
        private async Task<int> LoadSubscriptionsList() {


            if (_activeSubscriptions.Count == 0)
            {
                return 0;
            }
            else if (this._currenciesData.Length == 0)
            {
                return 1;
            }

            foreach (var item in this._currenciesData)
            {
                Pixbuf icon;

                if (this.ExistResource(new string[] { "src", "assets", "icons", "currency", item[CryptoCurrencyKeys.Symbol].ToLower() + ".png" })) {
                    icon = Pixbuf.LoadFromResource($"CryptoTrackApp.src.assets.icons.currency.{item[CryptoCurrencyKeys.Symbol].ToLower()}.png");
                }
                else {
                    icon = Pixbuf.LoadFromResource("CryptoTrackApp.src.assets.icons.currency.not_found.png");
                }

                this._subsTree.AddData
                (
                    icon,
                    item[CryptoCurrencyKeys.Name],
                    int.Parse(item[CryptoCurrencyKeys.Rank]),
                    double.Parse(item[CryptoCurrencyKeys.PriceUsd]),
                    float.Parse(item[CryptoCurrencyKeys.ChangePercent24Hr]),
                    float.Parse(_activeSubscriptions.First(s => s[SubscriptionKeys.CurrencyId] == item[CryptoCurrencyKeys.Id])[SubscriptionKeys.NotificationThreshold]),
                    item[CryptoCurrencyKeys.Id]
                );
            }

            return 2;

        }

        private void ConfigSpinner() {
                this._spinner.Expand = true;
                this._spinner.Hexpand = true;
                this._spinner.Visible = true;
                this._spinner.HeightRequest = 80;
                this._spinner.WidthRequest = 80;
                this._spinner.Halign = Align.Center;
                this._spinner.Valign = Align.Center;
        }




        private void ShowMessagePanel(string pMessage, string pImagePath)
        {
            if (this._panelMessage.Children.Length == 0) {
                // Widget Stylization
                FontDescription fontDesc = new FontDescription {
                    Family = "Arimo",
                    Size = (int)(18 * Pango.Scale.PangoScale),
                    Weight = Weight.Bold
                };
                
                Label message = new Label {
                    Halign = Align.Center, Valign = Align.End, Hexpand = true,
                    Vexpand = true, Justify = Justification.Center
                };

                message.OverrideFont(fontDesc);
                message.StyleContext.AddClass("important-label");

                Image img = new Image { Vexpand = true };

                this._panelMessage.Add(message);
                this._panelMessage.Add(img);
            }

            var label = this._panelMessage.Children[0] as Label;
            var image = this._panelMessage.Children[1] as Image;

            if (label != null) {
                label.Text = pMessage;
            }

            if (image != null && !string.IsNullOrEmpty(pImagePath)) {
                image.File = pImagePath;
            }

            this._spinner.Hide();
            this._panelScroll.Hide();
            this._panelMessage.ShowAll();
        }


        private void FollowButtonReleased (object sender, ButtonReleaseEventArgs args)
        {
            _logger.Log("[EVENT - FollowButtonReleased at MainView] - Delegated to ViewManager");
            ViewManager vw = ViewManager.GetInstance();
            vw.ShowView(ViewsIds.Follow);

        }

        private async void LoadBoxPlot (string pCurrencyId = "") {

            foreach (Widget widget in _panelBoxPlot.Children) {
                widget.Destroy();
            }

            Spinner spinner = new();
            spinner.Expand = true;
            spinner.Hexpand = true;
            spinner.HeightRequest = 80;
            spinner.WidthRequest = 80;
            spinner.Halign = Align.Center;
            spinner.Valign = Align.Center;
            spinner.Active = true;
            spinner.Visible = true;
            spinner.Show();
            _panelBoxPlot.Add(spinner);
            _panelBoxPlot.ReorderChild(spinner, 1);
            _panelBoxPlot.ShowAll();
            

            List<(DateTime, double)> historyValues = new List<(DateTime, double)>();
            string plotPath = "";
            Label emptyLbl = new();

            bool shouldShowPlot = false;

            if (_currenciesData != null && _currenciesData.Length > 0)
            {
                string currencyId = pCurrencyId != ""
                ? pCurrencyId
                : _currenciesData[0][CryptoCurrencyKeys.Id];

                _logger.Log($"[EXECT - Operation LoadBoxPlot at MainView - Parameters: [currency: {currencyId}]]");
                if (currencyId == "")
                {

                    _logger.Log($"[FAILURE - Operation LoadBoxPlot at MainView - Couldn't load plot - Parameters: [currency: {currencyId}]]");
                    emptyLbl.Text = "👉🏻 Select a currency of the following table to display data. ";
                    currencyId = DefaultCurrencyData.bitcoinId;
                    historyValues = await this._currencyService.GetHistory(pCurrencyId: currencyId);
                }
                else
                {
                    historyValues = await this._currencyService.GetHistory(pCurrencyId: currencyId);
                }


                if (currencyId != "" && historyValues.Count == 0)
                { // Currency selected, couldn't get history.

                    _logger.Log("[FAILURE - Operation LoadBoxPlot at MainView - Couldn't get the history values to plot]");
                    emptyLbl.Text = "❌ Error while trying to get currency values history..., try again or contact support.";
                }
                else if (currencyId != "" && historyValues.Count > 0)
                {

                    int plotWidth;
                    int plotHeigh;
                    this.GetSize(out plotWidth, out plotHeigh);
                    plotWidth = (int)Math.Round(plotWidth - plotWidth * 0.20);
                    plotHeigh = (int)Math.Round(plotHeigh * 0.5);
                    IDictionary<string, string>? currency = _currenciesData.FirstOrDefault(x => x["Id"] == currencyId);
                    string currencyName = currency == null ? DefaultCurrencyData.bitcoinName : currency["Name"];
                    plotPath = await this._plotService.GetCandlesPlot(
                        historyValues,
                        width: plotWidth,
                        height: plotHeigh,
                        $"{currencyName} value over time"
                    );
                }

            }

            if (historyValues.Count > 0 && plotPath == "")
            {
                _logger.Log("[FAILURE - Operation LoadBoxPlot at MainView - Get the plot for the currency history]");
                emptyLbl.Text = "❌ Error while plotting values history for the currency..., try again or contact support.";
            }
            else if (historyValues.Count > 0 && plotPath != "")
            {
                shouldShowPlot = true;
            }


            if (shouldShowPlot) {
                Image boxplot = new();
                boxplot.File = plotPath;
                boxplot.StyleContext.AddClass("boxplot-image");
                spinner.Active = false;
                spinner.Destroy();
                _panelBoxPlot.Add(boxplot);
                _panelBoxPlot.ReorderChild(boxplot, 1);
            }
            else {
                emptyLbl.Hexpand = true;
                spinner.Active = false;
                spinner.Destroy();
                _panelBoxPlot.Add(emptyLbl);
                _panelBoxPlot.ReorderChild(emptyLbl, 1);
            }

            _panelBoxPlot.ShowAll();

        }

        private async Task ResetSubsTree()
        {

            this._subsTree!.Destroy();
            this._subsTree = null;
            await this.LoadTablePanel();
            
        }
    }


}