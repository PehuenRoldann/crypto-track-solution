using System;
using Gtk;
using Gdk;
using Pango;
using CryptoTrackApp.src.view.Utils;
using CryptoTrackApp.src.utils;
using IO = System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace CryptoTrackApp.src.view.Components
{
    public class ConfirmationDialog : Gtk.Dialog
    {
        public Button ConfirmationBtn {get; private set;}
        private Button CancelBtn;


        public ConfirmationDialog(Gtk.Window pParent, string pTitle, string pMessage, int pWidth = 400,
            int pHeight = 300, string[]? pImgPathArr = null, DialogFlags pFlags = DialogFlags.Modal) : base(pTitle, pParent, pFlags)
        {

            // Configuración del mensaje
            Label label = new Label(pMessage);
            label.Justify = Justification.Center;
            label.PangoContext.FontDescription = FontDescription.FromString("Arial 16");
            label.Expand = true;

            
            // Configuramos la imagen si se pasa una
            string basePath = "";
            string imgPath = "";
            bool isValidExtension = false;
            bool imageExists = false;
            if (pImgPathArr != null && pImgPathArr.Length > 0) {
                
                basePath = AppDomain.CurrentDomain.BaseDirectory;
                imgPath = IO.Path.Combine(pImgPathArr.Prepend(basePath).ToArray());
            }

            if (basePath != imgPath) {
                imageExists = IO.File.Exists(imgPath);
                string imagePattern = @"\.(png|jpg|jpeg|bmp|gif|svg)$";
                isValidExtension = Regex.IsMatch(imgPath, imagePattern, RegexOptions.IgnoreCase);
            }

            if (imageExists && isValidExtension)
            {
                Gtk.Image image = new Gtk.Image(imgPath);
                int imageSize = (int)(pHeight * 0.1);
                image.SetSizeRequest(imageSize, imageSize);
                this.ContentArea.Add(image);
            }

            // Configuración de los botones
            ConfirmationBtn = new Button { Label = "PROCEED" };
            CancelBtn = new Button { Label = "CANCEL" };

            // Configuración de Handlers y eventos
             CancelBtn.Released += (sender, e) => {
                this.Destroy();
            };

            // Crear un contenedor horizontal para los botones
            HButtonBox buttonBox = new HButtonBox();
            buttonBox.Layout = ButtonBoxStyle.End; // Alinea los botones a la derecha
            buttonBox.Spacing = 6; // Espacio entre los botones
            buttonBox.Add(ConfirmationBtn);
            buttonBox.Add(CancelBtn);
            buttonBox.Hexpand = true;
            buttonBox.Halign = Align.Center;

            // Configuración del diálogo
            this.SetDefaultSize(pWidth, pHeight);
            this.ContentArea.Expand = true;

            // Agregar el mensaje y el contenedor de botones al diálogo
            this.ContentArea.Add(label);
            this.ContentArea.Add(buttonBox);


            // Set Styles
            this.StyleContext.AddClass(ConfrimationDialogCssClases.ConfrimationDialog);
            CancelBtn.StyleContext.AddClass(ConfrimationDialogCssClases.CancelBtn);
            CancelBtn.StyleContext.AddClass(ConfrimationDialogCssClases.DialogBtn);
            ConfirmationBtn.StyleContext.AddClass(ConfrimationDialogCssClases.ConfirmationBtn);
            ConfirmationBtn.StyleContext.AddClass(ConfrimationDialogCssClases.DialogBtn);

            ConfirmationBtn.CanFocus = false; // Esto evita el focus por defecto
            CancelBtn.CanFocus = false; 

            var pangoDescBtns = new FontDescription {
                    Family = "Arimo",
                    Size = 12
                };

            CancelBtn.PangoContext.FontDescription = pangoDescBtns;
            ConfirmationBtn.PangoContext.FontDescription = pangoDescBtns;

            this.Resizable = false;
                

            // Mostrar todos los widgets
            this.ShowAll();
        }

    }
}   