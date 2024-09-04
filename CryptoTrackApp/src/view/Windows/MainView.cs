using System;
using IO = System.IO;
using System.Collections.Generic;
using Gtk;
using Gdk;
using UI = Gtk.Builder.ObjectAttribute;
using CryptoTrackApp.src.view.Components;
using CryptoTrackApp.src.view.Controllers;
using CryptoTrackApp.src.services;
using System.Threading.Tasks;
using Pango;
using CryptoTrackApp.src.db;
using Npgsql.Replication.PgOutput.Messages;


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
        // [UI] private ScrolledWindow _scrolledWindow;
        [UI] private Box _panelScroll;

        [UI] private AspectFrame _panelTable;

        // private Spinner _spinner = new Spinner();
        [UI] private Spinner _spinner;
        private Label _message;
        private Box _noSubscriptionsBox;
        private string _userId;
        private CryptoTreeViewComponent subsTree;
        private ISubscriptionServices subscriptionService;
        private ICurrencyServices currencyService;

        private string[] LOGOUT_IMAGE_PATH = {"src", "assets", "icons", "logout.png"};
        private string[] LOGO_PATH = {"src", "assets", "images", "cta_logo_200x200.png"};
        // private string NOT_FOUND_PATH = "./src/assets/images/not_found.png";
        private string[] NOT_FOUND_PATH = {"src", "assets", "images", "not_found.png"};
        // private string SERVER_BURNING_PATH = "./src/assets/images/server_burning.png";
        private string[] SERVER_BURNING_PATH = {"src", "assets", "images", "server_burning.png"};
        private string[] CSS_PATH = {"src", "css", "main_view.css"};


        public MainView(
            string pUserId,
            ISubscriptionServices pSubService,
            ICurrencyServices pCurrencyService) : base("MainView")
        {
            this._userId = pUserId;
            this.subscriptionService = pSubService;
            this.currencyService = pCurrencyService;
            
            this.InitPanel();
            this.SetStyle(this.GetAbsolutePath(CSS_PATH));

        }

// ----------- INITIAL CONFIGURATIONS ---------------------------------------
        private void OnDelete(object o, DeleteEventArgs args)
        {
            Application.Quit();
        }

        public override void ConfigEventHandlers()
        {
            Console.WriteLine("Configuring event handlers.......");
            this._followButton.ButtonReleaseEvent += FollowButtonReleased;
            this._panelBtn.ButtonReleaseEvent += PanelButtonReleased;
        }

        public override void ConfigImages()
        {
            this._logoImg.File = this.GetAbsolutePath(LOGO_PATH);
            this._logoutBtnImg.File = this.GetAbsolutePath(LOGOUT_IMAGE_PATH);
            Console.WriteLine("Cargando imagenes");
            Console.WriteLine("Configuring images...");

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
            ConfigSpinner();
            this._spinner.Active = true;
            this._panelMessage.Hide();
            this._spinner.Show();
            LoadTablePanel();
 
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
                        // this._message = this.Initmessage();
                        ShowMessagePanel(
                            pMessage: "You are not following any crypto yet!\n"+
                            "Press the follow button in the navbar to start following some currencies.",
                            pImagePath: this.GetAbsolutePath(NOT_FOUND_PATH)
                        );
                    break;
                    case 1:
                        // Console.WriteLine("Error: " + error.GetType());
                        ShowMessagePanel(
                            pMessage: "This application uses a third party API to get the currencies info.\n"
                            +"Please, wait a few seconds and try again.", 
                            pImagePath: this.GetAbsolutePath(SERVER_BURNING_PATH)
                        );
                    break;
                    case 2:
                        Console.WriteLine("Añadienddo sbus tree...");
                        this._panelScroll.Add(this.subsTree);
                        // this._panel.ReorderChild(this.subsTable, 1);
                        // this._panelTable = this.subsTable!;
                        this._spinner.Hide();
                        this._panelMessage.Hide();
                        // this._panelTable.ShowAll();
                        Console.WriteLine("Monstrar subs tree");
                        this._panelScroll.ShowAll();
                    break;
                }

        }

        public void ConfigSubsList()                    /* this._panel.Add(this._message);
                    this._panel.ReorderChild(this._message, 1);
                    this._spinner.Hide();
                    this._message.Show(); */
        {
            CryptoTreeViewComponent subsTree = new CryptoTreeViewComponent();
            subsTree.Expand = true;
            subsTree.Halign = Align.Center;
            subsTree.Valign = Align.Center;
            subsTree.StyleContext.AddClass("subs-tree");
            this.subsTree = subsTree;

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
            
            List<string> cryptosId = await subscriptionService.GetFollowedCryptosIdsAsync(this._userId);
            // List<string> cryptosId = await subscriptionService.GetFollowedCryptosIdsAsync("4d266202-d63e-4caf-a87f-6ef56e0dd1b6");
            if (cryptosId.Count == 0)
            {
                return 0;
            }

            IDictionary<string,string>[] currenciesData;

            currenciesData =  await currencyService.GetCurrencies(cryptosId.ToArray());

            if (currenciesData.Length == 0) {
                return 1;
            }

            foreach (var item in currenciesData)
            {
                Pixbuf icon;

                if (this.ExistResource(new string[] {"src", "assets", "icons", "currency", item["Symbol"].ToLower() + ".png"})) {
                    icon = Pixbuf.LoadFromResource($"CryptoTrackApp.src.assets.icons.currency.{item["Symbol"].ToLower()}.png");
                }
                else {
                    icon = Pixbuf.LoadFromResource("CryptoTrackApp.src.assets.icons.currency.not_found.png");
                }

                    this.subsTree.AddData(
                    icon,
                    item["Name"],
                    int.Parse(item["Rank"]),
                    double.Parse(item["PriceUsd"]),
                    float.Parse(item["ChangePercent24Hr"])
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
            // this._panelTable.Hide();
           /*  if (this.subsTree != null) {
                this.subsTree.Hide();
            } */
            Console.WriteLine("Se detuvo el spinner!");
            this._panelMessage.ShowAll();
        }


        private void FollowButtonReleased (object sender, ButtonReleaseEventArgs args)
        {
            ViewManager vw = ViewManager.GetInstance();
            vw.ShowView("follow");
        }
    }


}