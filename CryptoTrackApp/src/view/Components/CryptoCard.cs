using System;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;
using System.Threading.Tasks;
using CryptoTrackApp.src.services;
using CryptoTrackApp.src.view.Controllers;

namespace CryptoTrackApp.src.view.Components
{
    public class CryptoCard : Gtk.Box
    {
        [UI] private Button _followButton;
        public string CryptoName {get; set; }
        public string CryptoId {get; set; }
        public bool AlreadyFollow {get; set; }
        private ISubscriptionServices _subService;

        public CryptoCard (ISubscriptionServices pSubscriptionService, string pCryptoSymbol = "",
            string pCryptoName = "", string pCryptoId = "", bool pAlreadyFollow = false)
        {
            this._subService = pSubscriptionService;
            this.CryptoName = pCryptoName;
            this.CryptoId = pCryptoId;
            this.AlreadyFollow = pAlreadyFollow;

            this.WidthRequest = 100;
            this.HeightRequest = 200;
            this.Expand = false;
            this.Orientation = Orientation.Vertical;
            
            var name_label = new Gtk.Label(pCryptoName);

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

            var info_btn = new Gtk.Button("+Info");
            info_btn.StyleContext.AddClass("card-button");
            info_btn.StyleContext.AddClass("info-button");

            Gdk.Pixbuf pixbuf;
            // Cargar la imagen desde un archivo
            try
            {
                pixbuf = new Gdk.Pixbuf($"./src/assets/images/currency/{pCryptoSymbol}.svg");
            }
            catch (Exception _error) {
                pixbuf = new Gdk.Pixbuf($"./src/assets/images/currency/not_found.svg");
            }

            // Escalar el Pixbuf al nuevo tamaño
            var scaledPixbuf = pixbuf.ScaleSimple(100, 100, Gdk.InterpType.Bilinear);

            // Crear una nueva imagen con el Pixbuf escalado
            var image = new Image(scaledPixbuf);
            image.StyleContext.AddClass("card-image");
            // Añadir la imagen al contenedor vertical
            this.PackStart(name_label, false, false, 0);
            this.PackStart(image, false, false, 0);
            this.PackEnd(this._followButton, false, false, 0);
            this.PackEnd(info_btn, false, false, 0);

            ShowAll();
        }

        private async void FollowButtonReleased (object sender, ButtonReleaseEventArgs args)
        {
            Console.WriteLine($"Follow button from {this.CryptoId} pressed.");
            try
            {
                if (this._followButton.Label == "Follow")
                {
                    await Task.Run(() => {
                        this._subService.AddSubscriptionAsync(ViewManager.GetInstance().UserId,
                        this.CryptoId);
                    });
                    this._followButton.Label = "Unfollow";
                    this._followButton.StyleContext.RemoveClass("follow-button");
                    this._followButton.StyleContext.AddClass("unfollow-button");
                    // this._followButton.CanFocus = false;
                }
                else
                {
                    Console.WriteLine("Tried to unfollow but the feature isn't implemented");
                    this._followButton.Label = "Follow";
                    this._followButton.StyleContext.RemoveClass("unfollow-button");
                    this._followButton.StyleContext.AddClass("follow-button");
                    // this._followButton.Label = "Follow";
                    // this._followButton.CanFocus = false;
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
            }
        }
    }
}