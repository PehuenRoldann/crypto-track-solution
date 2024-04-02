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
        private ISubscriptionServices _subService;

        public CryptoCard (ISubscriptionServices pSubscriptionService, string pCryptoSymbol = "",
            string pCryptoName = "", string pCryptoId = "")
        {
            this._subService = pSubscriptionService;
            this.CryptoName = pCryptoName;
            this.CryptoId = pCryptoId;
            
            this.WidthRequest = 100;
            this.HeightRequest = 200;
            this.Expand = false;
            this.Orientation = Orientation.Vertical;
            
            var name_label = new Gtk.Label(pCryptoName);

            this._followButton = new Gtk.Button("Follow");
            this._followButton.ButtonReleaseEvent += FollowButtonReleased;

            var info_btn = new Gtk.Button("+Info");

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
                await Task.Run(() => {
                    this._subService.AddSubscriptionAsync(ViewManager.GetInstance().UserId, this.CryptoId);
                });
                this._followButton.Label = "Already following";
                this._followButton.CanFocus = false;
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
            }
        }
    }
}