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


namespace CryptoTrackApp.src.view.Windows
{
    public class MainView : View
    {
        [UI] private Button _followButton;
        [UI] private Image _logoImg;
        [UI] private Box _panel;
        private Spinner _spinner = new Spinner();
        private Label _warning;
        private string _userId;
        private CryptoTreeViewComponent subsTable;
        private ISubscriptionServices subscriptionService;
        private ICurrencyServices currencyService;
        

        private string LOGO_PATH = "./src/assets/images/logo_v2.png";
        public MainView(
            string pUserId,
            ISubscriptionServices pSubService,
            ICurrencyServices pCurrencyService) : base("MainView")
        {
            this._userId = pUserId;
            this.subscriptionService = pSubService;
            this.currencyService = pCurrencyService;
            this.ConfigNotificationsArea();
            this.InitPanel();
            DeleteEvent += OnDelete;
            /* this.CSS_PATH_DARK = "./src/css/main_view.css"; */
            this.SetStyle("./src/css/main_view.css");
            this._panel.ShowAll();
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
            Console.WriteLine("Configuring images...");

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

        public void ConfigNotificationsArea()
        {
            NotificationsAreaComponent notifArea = new NotificationsAreaComponent();
            notifArea.Vexpand = true;
            notifArea.StyleContext.AddClass("notification-area");
            this._panel.Add(notifArea);
            this._panel.SetSizeRequest(300, -1);
        }

        private async Task<bool> LoadSubscriptionsList(){
            List<string> cryptosId = await subscriptionService.GetFollowedCryptosIdsAsync(this._userId);
            // List<string> cryptosId = await subscriptionService.GetFollowedCryptosIdsAsync("4d266202-d63e-4caf-a87f-6ef56e0dd1b6");
            if (cryptosId.Count == 0)
            {
                return false;
            }
            IDictionary<string,string>[] currenciesData =  await currencyService.GetCurrencies(cryptosId.ToArray());
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


        private async void InitPanel()
        {
            this._spinner.Expand = true;
            this._spinner.Visible = true;
            this._spinner.HeightRequest = 80;
            this._spinner.WidthRequest = 80;
            this._spinner.Halign = Align.Center;
            this._spinner.Valign = Align.Center;
            this._panel.Add(this._spinner);
            this._panel.ReorderChild(this._spinner, 1);
            this._spinner.Active = true;;
            this._spinner.Show();
            this.ConfigSubsList();
            /* await Task.Run(() => {
                this.LoadSubscriptionsList();
            }); */
            bool haveSubscriptions = await this.LoadSubscriptionsList();

            if (haveSubscriptions)
            {
                this._panel.Add(this.subsTable);
                this._panel.ReorderChild(this.subsTable, 1);
                this._spinner.Hide();
                this.subsTable.ShowAll();
            }
            else
            {
                this._warning = this.InitWarning();
                this._panel.Add(this._warning);
                this._panel.ReorderChild(this._warning, 1);
                this._spinner.Hide();
                this._warning.Show();
            }
            
        }

        private Label InitWarning()
        {
            FontDescription fontDesc = new FontDescription();
            fontDesc.Family = "Arimo";
            fontDesc.Size = (int)(18 * Pango.Scale.PangoScale);
            fontDesc.Weight = Weight.Bold;
            Label warning = new Label("You are not following any crypto yet!\n"+
            "Check press the follow button in the navbar to start following some currencies.");
            warning.OverrideFont(fontDesc);
            warning.Halign = Align.Center;
            warning.Valign = Align.Center;
            warning.Hexpand = true;
            warning.Justify = Justification.Center;
            return warning;
        }

        private void FollowButtonReleased (object sender, ButtonReleaseEventArgs args)
        {
            ViewManager vw = ViewManager.GetInstance();
            vw.ShowView("follow");
        }
    }


}