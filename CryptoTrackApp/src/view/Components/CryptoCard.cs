using System;
using Gtk;

namespace CryptoTrackApp.src.view.Components
{
    public class CryptoCard : Gtk.Box
    {
        public string CryptoName;
        public string CryptoId;

        public CryptoCard (string pCryptoSymbol = "btc", string pCryptoName = "Bitcoin",
         string pCryptoId = "") {

            this.CryptoName = pCryptoName;
            this.CryptoId = pCryptoId;
            // SetDefaultSize(400, 200);
            this.WidthRequest = 100;
            this.HeightRequest = 200;
            this.Expand = false;
            this.Orientation = Orientation.Vertical;
            
            var name_label = new Gtk.Label(pCryptoName);

            var follow_btn = new Gtk.Button("Follow");
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
            this.PackEnd(follow_btn, false, false, 0);
            this.PackEnd(info_btn, false, false, 0);

            ShowAll();
        }
    }
}