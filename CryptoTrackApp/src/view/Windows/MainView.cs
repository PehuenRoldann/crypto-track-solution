using System;
using System.Collections.Generic;
using Gtk;
using Gdk;
using UI = Gtk.Builder.ObjectAttribute;
using CryptoTrackApp.src.view.Components;
using CryptoTrackApp.src.services;
using System.Threading.Tasks;
using Pango;
using System.Linq;
using CryptoTrackApp.src.utils;
using System.Linq;

namespace CryptoTrackApp.src.view.Windows
{
    
    public class MainView : View
    {
        [UI] private Button _panelBtn;
        [UI] private Button _followButton;
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
        // [UI] private Image _drawingArea;

        [UI] private AspectFrame _panelTable;

        // private Spinner _spinner = new Spinner();
        [UI] private Spinner _spinner;
        private Label _message;
        private Box _noSubscriptionsBox;
        private string _userId;
        private CryptoTreeViewComponent subsTree;
        private ISubscriptionServices subscriptionService;
        private ICurrencyServices currencyService;
        private IPlotterService plotService;

        private IDictionary<string,string>[] currenciesData;

        private Logger _logger = new Logger();


        private const int PLOT_WIDTH = 900;
        private const int PLOT_HEIGHT = 400;

        private string[] LOGOUT_IMAGE_PATH = {"src", "assets", "images", "logout.png"};
        private string[] LOGO_PATH = {"src", "assets", "images", "cta_logo_64x64.png"};
        // private string NOT_FOUND_PATH = "./src/assets/images/not_found.png";
        private string[] NOT_FOUND_PATH = {"src", "assets", "images", "not_found.png"};
        private string[] PANEL_IMG_PATH = {"src", "assets", "images", "panel.png"};
        private string[] COMPAS_IMG_PATH = {"src", "assets", "images", "compass.png"};
        private string[] ABOUT_IMG_PATH = {"src", "assets", "images", "about.png"};
        // private string SERVER_BURNING_PATH = "./src/assets/images/server_burning.png";
        private string[] SERVER_BURNING_PATH = {"src", "assets", "images", "server_burning.png"};
        //private string[] CSS_PATH = {"src", "css", "main_view.css"};


        public MainView(
            string pUserId,
            ISubscriptionServices pSubService,
            ICurrencyServices pCurrencyService,
            IPlotterService pPlotService) : base(Templates.MainView)
        {
            _logger.Log("[INIT - MainView]");
            this._userId = pUserId;
            this.subscriptionService = pSubService;
            this.currencyService = pCurrencyService;
            this.plotService = pPlotService;
            
            this.InitPanel();
            this.SetStyle(CssFilesPaths.MainViewCss);

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
        }

        public override void ConfigImages()
        {
            this._logoImg.File = this.GetAbsolutePath(LOGO_PATH);
            this._logoutBtnImg.File = this.GetAbsolutePath(LOGOUT_IMAGE_PATH);
            this._panelBtnImg.File = this.GetAbsolutePath(PANEL_IMG_PATH);
            this._followBtnImg.File = this.GetAbsolutePath(COMPAS_IMG_PATH);
            this._aboutBtnImg.File = this.GetAbsolutePath(ABOUT_IMG_PATH);
        }

        private void PanelButtonReleased (object sender, ButtonReleaseEventArgs args) {
            this._panelScroll.Hide();
            this._panelMessage.Hide();
            this._spinner.Show();
            LoadTablePanel();
        }

        private async void InitPanel()
        {
            
            this._panel.Halign = Align.Start;
            this._middlePanel.Halign = Align.Center;
            ConfigSpinner();
            this._spinner.Active = true;
            /* this._panelMessage.Hide();
            this._panelBoxPlot.Hide(); */
            this._middlePanel.Hide();
            this._spinner.Show();
            LoadTablePanel();
            LoadBoxPlot();
 
        }


        /// <summary>
        /// Loads and show the 
        /// </summary>
        public async void LoadTablePanel () {

            if (this.subsTree == null) {
                ConfigSubsList();
            }

            int haveSubscriptions = await this.LoadSubscriptionsList();

                switch (haveSubscriptions) {
                    case 0:
                        ShowMessagePanel(
                            pMessage: "You are not following any crypto yet!\n"+
                            "Press the follow button in the navbar to start following some currencies.",
                            pImagePath: this.GetAbsolutePath(NOT_FOUND_PATH)
                        );
                    break;
                    case 1:
                        ShowMessagePanel(
                            pMessage: "This application uses a third party API to get the currencies info.\n"
                            +"Please, wait a few seconds and try again.", 
                            pImagePath: this.GetAbsolutePath(SERVER_BURNING_PATH)
                        );
                    break;
                    case 2:
                        this._panelScroll.Add(this.subsTree);
                        this._spinner.Hide();
                        this._panelMessage.Hide();
                        this._panelScroll.ShowAll();
                        this._middlePanel.ShowAll();
                    break;
                }

        }

        public void ConfigSubsList()
        {
            CryptoTreeViewComponent subsTree = new CryptoTreeViewComponent(heigh:350);
            subsTree.RowActivatedEvent += (sender, e) =>
            {
                _logger.Log($"[EVENT - RowAcitvatedEvent - Row values: [Name: {e.Name}, Rank: {e.Rank}, UsdPrice: {e.UsdPrice}, Tendency: {e.Tendency}]]");
                IDictionary<string, string> currency = this.currenciesData.First<IDictionary<string, string>>(elem => elem["Name"] == e.Name);
                this.LoadBoxPlot(currency["Id"]);
            };

            subsTree.UnfollowEvent += UnfollowPressedEventHandler;

            subsTree.Expand = true;
            subsTree.Halign = Align.Center;
            subsTree.Valign = Align.Center;
            subsTree.StyleContext.AddClass(MainViewClases.SubsTree);
            this.subsTree = subsTree;

        }


        public void UnfollowPressedEventHandler (object sender, UnfollowEventArgs e) {

            
            var imgPath = AssetsArrPaths.CurrenciesImages;
            var currencySymbol = this.currenciesData.First(c => c["Id"] == e.CurrencyId)["Symbol"];
            var unfollowDialog = new ConfirmationDialog(
                this,
                "Unfollow",
                $"Are you sure about unfollow {e.Name}?",
                pImgPathArr: imgPath.Append($"{currencySymbol.ToLower()}.svg").ToArray()
            );


            unfollowDialog.ConfirmationBtn.Released += (s, a) => {
                unfollowDialog.Destroy();
                IViewManager vm = ViewManager.GetInstance();
                this.subscriptionService.UnfollowAsync(_userId, e.CurrencyId);
            };

            unfollowDialog.ShowAll();

        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 0: No subscriptions.
        /// 1: There are subscriptions, but there was a problem getting the info.
        /// 2: The information has ben retrived successfully.
        /// </returns>
        private async Task<int> LoadSubscriptionsList(){
            
            // List<string> cryptosId = await subscriptionService.GetFollowedCryptosIdsAsync(this._userId);
            
            List<IDictionary<string, string>>? subscriptionsList = await subscriptionService.GetSubscriptionsAsync(this._userId);
            List<string> cryptosId = new List<string>();

            if (subscriptionsList != null) {
                cryptosId = subscriptionsList.Select(s => s["CurrencyId"]).ToList();
            }

            if (cryptosId.Count == 0)
            {
                return 0;
            }


            this.currenciesData =  await currencyService.GetCurrencies(cryptosId.ToArray());

            if (this.currenciesData.Length == 0) {
                return 1;
            }

            foreach (var item in this.currenciesData)
            {
                Pixbuf icon;

                if (this.ExistResource(new string[] {"src", "assets", "icons", "currency", item["Symbol"].ToLower() + ".png"})) {
                    icon = Pixbuf.LoadFromResource($"CryptoTrackApp.src.assets.icons.currency.{item["Symbol"].ToLower()}.png");
                }
                else {
                    icon = Pixbuf.LoadFromResource("CryptoTrackApp.src.assets.icons.currency.not_found.png");
                }

                this.subsTree.AddData
                (
                    icon,
                    item["Name"],
                    int.Parse(item["Rank"]),
                    double.Parse(item["PriceUsd"]),
                    float.Parse(item["ChangePercent24Hr"]),
                    float.Parse(subscriptionsList.First(s => s["CurrencyId"] == item["Id"])["NotificationUmbral"]),
                    item["Id"]
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
            vw.ShowView("follow");
        }

        private async void LoadBoxPlot (string currency = "" ) {

            _logger.Log($"[EXECT - Operation LoadBoxPlot at MainView - Parameters: [currency: {currency}]]");

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
            
            await Task.Delay(2000);

            List<(DateTime, double)> historyValues = new List<(DateTime, double)>();
            string plotPath = "";
            Label emptyLbl = new();

            bool shouldShowPlot = false;

            if (currency == "") {

                _logger.Log($"[FAILURE - Operation LoadBoxPlot at MainView - Couldn't load plot - Parameters: [currency: {currency}]]");
                emptyLbl.Text = "Select a currency of the following table to display data.";
                currency = "bitcoin";
                historyValues = await this.currencyService.GetHistory(pCurrencyId: currency);
            }
            else {
                historyValues = await this.currencyService.GetHistory(pCurrencyId:currency);
            }


            if ( currency != "" && historyValues.Count == 0) { // Currency selected, couldn't get history.

                _logger.Log("[FAILURE - Operation LoadBoxPlot at MainView - Couldn't get the history values to plot]");
                emptyLbl.Text = "Error while trying to get currency values history..., try again or contact support.";
            }
            else if ( currency != "" && historyValues.Count > 0 ) {

                int plotWidth;
                int plotHeigh;
                this.GetSize(out plotWidth, out plotHeigh);
                plotWidth = (int)Math.Round(plotWidth - plotWidth * 0.20);
                plotHeigh = (int)Math.Round(plotHeigh * 0.5);
                string currencyName =  currenciesData.First(x => x["Id"] == currency)["Name"];
                plotPath = await this.plotService.GetCandlesPlot(historyValues, width: plotWidth, height: plotHeigh, $"{currencyName} value over time");
            }


            if ( historyValues.Count > 0 && plotPath == "" ) {
                _logger.Log("[FAILURE - Operation LoadBoxPlot at MainView - Get the plot for the currency history]");
                emptyLbl.Text = "Error while plotting values history for the currency..., try again or contact support.";
            }
            else if (historyValues.Count > 0 && plotPath != "")  {
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
               // emptyLbl.Text = "Select a currency of the following table to display data."; // sacar después
                emptyLbl.Hexpand = true;
                spinner.Active = false;
                spinner.Destroy();
                _panelBoxPlot.Add(emptyLbl);
                _panelBoxPlot.ReorderChild(emptyLbl, 1);
            }

            _panelBoxPlot.ShowAll();

        }



        private void RowActivatedEventHandler (object sender, CryptoRowActivatedEventArgs args) {

            Console.WriteLine("Clicked row with: ", args.Name);
        }
    }


}