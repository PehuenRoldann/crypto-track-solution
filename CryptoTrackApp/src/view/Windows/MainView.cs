using System;
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
        [UI] private Button _followButton;
        [UI] private Image _logoImg;
        [UI] private Image _logoutBtnImg;
        [UI] private Box _panel;
        [UI] private Box _panelMessage;

        [UI] private AspectFrame _panelTable;

        // private Spinner _spinner = new Spinner();
        [UI] private Spinner _spinner;
        private Label _message;
        private Box _noSubscriptionsBox;
        private string _userId;
        private CryptoTreeViewComponent subsTable;
        private ISubscriptionServices subscriptionService;
        private ICurrencyServices currencyService;

        private string LOGOUT_IMAGE_PATH = "./src/assets/icons/logout.png";
        private string LOGO_PATH = "./src/assets/images/cta_logo_200x200.png";
        private string NOT_FOUND_PATH = "./src/assets/images/not_found.png";


        public MainView(
            string pUserId,
            ISubscriptionServices pSubService,
            ICurrencyServices pCurrencyService) : base("MainView")
        {
            this._userId = pUserId;
            this.subscriptionService = pSubService;
            this.currencyService = pCurrencyService;
            
            this.InitPanel();
            // DeleteEvent += OnDelete;
            /* this.CSS_PATH_DARK = "./src/css/main_view.css"; */
            this.SetStyle("./src/css/main_view.css");
            //this._panel.ShowAll();
            //this._panelMessage.Hide();
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
        }

        public override void ConfigImages()
        {
            this._logoImg.File = this.LOGO_PATH;
            this._logoutBtnImg.File = this.LOGOUT_IMAGE_PATH;
            Console.WriteLine("Cargando imagenes");
            Console.WriteLine("Configuring images...");

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

            if (this.subsTable == null) {
                ConfigSubsList();
            }

            try {
                
                bool haveSubscriptions = await this.LoadSubscriptionsList();

                if (haveSubscriptions)
                {
                    this._panel.Add(this.subsTable);
                    this._panel.ReorderChild(this.subsTable, 1);
                    // this._panelTable = this.subsTable!;
                    this._spinner.Hide();
                    this._panelMessage.Hide();
                    // this._panelTable.ShowAll();
                    Console.WriteLine("Monstrar subs table");
                    this.subsTable.ShowAll();

                }
                else
                {
                    // this._message = this.Initmessage();
                    ShowMessagePanel(
                        pMessage: "You are not following any crypto yet!\n"+
                        "Press the follow button in the navbar to start following some currencies.",
                        pImagePath: this.NOT_FOUND_PATH
                    );
                    /* this._panel.Add(this._message);
                    this._panel.ReorderChild(this._message, 1);
                    this._spinner.Hide();
                    this._message.Show(); */
                }

            } catch (Exception error ) {
                Console.WriteLine("Error: " + error.GetType());
            }

            // List<string> cryptosId = await subscriptionService.GetFollowedCryptosIdsAsync("4d266202-d63e-4caf-a87f-6ef56e0dd1b6");

        }

        public void ConfigSubsList()
        {
            CryptoTreeViewComponent subsTree = new CryptoTreeViewComponent();
            /* this._panel.Add(subsTree);
            this._panel.ReorderChild(subsTree, 1); */
            subsTree.Expand = true;
            subsTree.Halign = Align.Center;
            subsTree.Valign = Align.Center;
            subsTree.StyleContext.AddClass("subs-tree");
            this.subsTable = subsTree;

        }

        private async Task<bool> LoadSubscriptionsList(){
            
            List<string> cryptosId = await subscriptionService.GetFollowedCryptosIdsAsync(this._userId);
            // List<string> cryptosId = await subscriptionService.GetFollowedCryptosIdsAsync("4d266202-d63e-4caf-a87f-6ef56e0dd1b6");
            if (cryptosId.Count == 0)
            {
                return false;
            }

            IDictionary<string,string>[] currenciesData;

            currenciesData =  await currencyService.GetCurrencies(cryptosId.ToArray());
            foreach (var item in currenciesData)
            {
                Pixbuf icon;
                try 
                {
                    icon = Pixbuf.LoadFromResource($"CryptoTrackApp.src.assets.icons.currency.{item["Symbol"].ToLower()}.png");
                }
                catch (Exception error)
                {
                    icon = Pixbuf.LoadFromResource("CryptoTrackApp.src.assets.icons.currency.not_found.png");
                }
                    this.subsTable.AddData(
                    icon,
                    item["Name"],
                    int.Parse(item["Rank"]),
                    double.Parse(item["PriceUsd"]),
                    float.Parse(item["ChangePercent24Hr"])
                );
            }

                return true;
            
        }

        private void ConfigSpinner() {
                // this._spinner = new Spinner();
                this._spinner.Expand = true;
                this._spinner.Hexpand = true;
                this._spinner.Visible = true;
                this._spinner.HeightRequest = 80;
                this._spinner.WidthRequest = 80;
                this._spinner.Halign = Align.Center;
                this._spinner.Valign = Align.Center;
                //this._panel.Add(this._spinner);
                // this._panel.ReorderChild(this._spinner, 1);
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
            // this._panelTable.Hide();
            if (this.subsTable != null) {
                this.subsTable.Hide();
            }
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