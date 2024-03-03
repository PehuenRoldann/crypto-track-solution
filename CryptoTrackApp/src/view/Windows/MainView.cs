using System;
using System.Collections.Generic;
using Gtk;
using Gdk;
using UI = Gtk.Builder.ObjectAttribute;
using CryptoTrackApp.src.view.Components;
using CryptoTrackApp.src.services;
using Pango;


namespace CryptoTrackApp.src.view.Windows
{
    public class MainView : View
    {
        [UI] private Image _logoImg;
        [UI] private Box _panel;
        private CryptoTreeViewComponent subsTable;
        private ISubscriptionServices subscriptionService = new SubscriptionServices();
        private ICurrencyServices currencyService = new CurrencyServices();

        private string LOGO_PATH = "./src/assets/images/logo_v2.png";
        public MainView() : base("MainView")
        {
            this.ConfigNotificationsArea();
            this.ConfigSubsList();
            this.LoadSubscriptionsList();
            DeleteEvent += OnDelete;
            this.CSS_PATH_DARK = "./src/css/main_view.css";
            this.SetStyle("dark");
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
        }

        public override void ConfigImages()
        {
            this._logoImg.File = this.LOGO_PATH;
            Console.WriteLine("Configuring images...");

        }

        public void ConfigSubsList()
        {
            CryptoTreeViewComponent subsTree = new CryptoTreeViewComponent();
            this._panel.Add(subsTree);
            this._panel.ReorderChild(subsTree, 1);
            subsTree.Expand = true;
            subsTree.Halign = Align.Center;
            subsTree.Valign = Align.Center;
            subsTree.StyleContext.AddClass("subs-tree");
            this.subsTable = subsTree;

        }

        public void ConfigNotificationsArea()
        {
            NotificationsAreaComponent notifArea = new NotificationsAreaComponent();
            notifArea.StyleContext.AddClass("notification-area");
            this._panel.Add(notifArea);
            this._panel.SetSizeRequest(300, -1);
        }

        private async void LoadSubscriptionsList(){

            List<string> cryptosId = await subscriptionService.GetFollowedCryptosIdsAsync("4d266202-d63e-4caf-a87f-6ef56e0dd1b6");
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
        }
    }
}