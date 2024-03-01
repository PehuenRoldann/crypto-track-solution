using System;
using Gtk;
using Gdk;
using Pango;
using CryptoTrackApp.src.view.Controllers;

namespace CryptoTrackApp.src.view.Windows
{
    public class MessageDialog : Gtk.Dialog
    {
        public Button Button {get; private set;}

        public MessageDialog(Gtk.Window pParent, string pTitle, string pMessage,
            Image pImage, string pButtonLabel, int pWidth = 400,
            int pHeight = 300, DialogFlags pFlags = DialogFlags.Modal) : base (pTitle, pParent, pFlags) {

                Image img;
            if (pImage != null) {
                img = pImage;
            }
            else {
                img = new Image(IconManager.not_found);
            }

            Label label = new Label(pMessage);
            label.Justify = Justification.Center;
            label.PangoContext.FontDescription = FontDescription.FromString("Arial 16");
            
            label.Expand = true;
            Button button = new Button();
            button.Label = pButtonLabel;
            //button.Expand = true;
            img.MarginTop = 30;
            this.SetDefaultSize(pWidth, pHeight);
            this.ContentArea.Expand = true;
            this.ContentArea.Add(img);
            this.ContentArea.Add(label);
            this.ContentArea.Add(button);
            this.Button = button;
        }
    }
}