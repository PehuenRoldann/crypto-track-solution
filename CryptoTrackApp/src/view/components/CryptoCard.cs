using System;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;
using System.Threading.Tasks;
using CryptoTrackApp.src.services;
using CryptoTrackApp.src.view.helpers;

namespace CryptoTrackApp.src.view.components
{
    public class CryptoCard : Box
    {
        [UI] private Button _followButton;
        public string CryptoName { get; set; }
        public string CryptoId { get; set; }
        public decimal PriceUsd { get; set; }
        public float ChangePercent24Hr { get; set; }
        public int Rank { get; set; }
        public bool AlreadyFollow { get; set; }
        private ISubscriptionServices _subService;

        public CryptoCard(
            ISubscriptionServices pSubscriptionService,
            string pCryptoSymbol = "",
            string pCryptoName = "",
            string pCryptoId = "",
            decimal pPriceUsd = 0,
            float pChangePercent24Hr = 0,
            int pRank = 0,
            bool pAlreadyFollow = false
            )
        {
            this._subService = pSubscriptionService;
            this.CryptoName = pCryptoName;
            this.CryptoId = pCryptoId;
            this.AlreadyFollow = pAlreadyFollow;
            this.Rank = pRank;
            this.ChangePercent24Hr = pChangePercent24Hr;
            this.PriceUsd = pPriceUsd;

            this.WidthRequest = 200;
            this.HeightRequest = 100;
            this.Expand = false;
            this.Hexpand= false;
            this.Vexpand = false;
            this.Valign = Align.Center;
            this.Halign = Align.Center;
            this.Orientation = Orientation.Vertical;

            var name_label = new Label(CryptoName);

            if (this.AlreadyFollow)
            {
                this._followButton = new Gtk.Button("Unfollow");
                this._followButton.StyleContext.AddClass("unfollow-button");
            }
            else
            {
                this._followButton = new Gtk.Button("Follow");
                this._followButton.StyleContext.AddClass("follow-button");
            }
            this._followButton.ButtonReleaseEvent += FollowButtonReleased;
            this._followButton.StyleContext.AddClass("card-button");

            var priceLabel = new Label($"💵 USD: {Math.Round(PriceUsd, 2)}");
            var changeLabel = new Label($"📈 24h: {Math.Round(ChangePercent24Hr,2)}%");
            var rankLabel = new Label($"🏅 Rank: {Rank}");


            Gdk.Pixbuf pixbuf;
            try
            {
                pixbuf = new Gdk.Pixbuf($"./src/assets/images/currency/{pCryptoSymbol}.svg");
            }
            catch (Exception _error)
            {
                pixbuf = new Gdk.Pixbuf($"./src/assets/images/currency/not_found.svg");
            }

            var scaledPixbuf = pixbuf.ScaleSimple(100, 100, Gdk.InterpType.Bilinear);
            var image = new Image(scaledPixbuf);
            image.StyleContext.AddClass("card-image");

            // Añadir los widgets al contenedor vertical sin expandirlos
            this.PackStart(name_label, false, false, 0);
            this.PackStart(image, false, false, 0);
            this.PackStart(priceLabel, false, false, 0);
            this.PackStart(changeLabel, false, false, 0);
            this.PackStart(rankLabel, false, false, 0);
            this.PackEnd(this._followButton, false, false, 0); 

            ShowAll();
        }

        private async void FollowButtonReleased(object sender, ButtonReleaseEventArgs args)
        {
            try
            {
                if (this._followButton.Label == "Follow")
                {
                    await Task.Run(() =>
                    {
                        this._subService.AddSubscriptionAsync(ViewManager.GetInstance().UserId,
                        this.CryptoId);
                    });
                    this._followButton.Label = "Unfollow";
                    this._followButton.StyleContext.RemoveClass("follow-button");
                    this._followButton.StyleContext.AddClass("unfollow-button");
                }
                else
                {
                    await Task.Run(() =>
                    {
                        this._subService.UnfollowAsync(ViewManager.GetInstance().UserId, this.CryptoId);
                    });
                    this._followButton.Label = "Follow";
                    this._followButton.StyleContext.RemoveClass("unfollow-button");
                    this._followButton.StyleContext.AddClass("follow-button");
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
            }
        }
    }
}
