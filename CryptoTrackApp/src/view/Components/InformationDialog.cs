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
    public class InformationDialog : Gtk.Dialog
    {
        private Label _message;
        private string[]? _imagePathArr = null;
        private Image _image;
        private Spinner _spinner = new Spinner();


        /// <summary>
        /// Creates a dialog with space for a message and a image.
        /// By the fault, it hiddes only shows the spinner until the method ShowContent is called.
        /// </summary>
        /// <param name="pParent"> Parent window </param>
        /// <param name="pTitle"> Dialog title </param>
        /// <param name="pWidth"> Dialog width (in px) </param>
        /// <param name="pHeight"> Dialog heigh (in px) </param>
        public InformationDialog(Gtk.Window pParent, string pTitle, int pWidth = 400,
            int pHeight = 280) : base(pTitle, pParent, DialogFlags.Modal)
        {

            // Configuración del mensaje
            _message = new Label("");
            _message.Justify = Justification.Center;
            _message.PangoContext.FontDescription = FontDescription.FromString("Arial 16");
            _message.Expand = true;

            _image = new Image();


            // Configuración del diálogo
            this.SetDefaultSize(pWidth, pHeight);
            this.ContentArea.Expand = true;

            // Configuracín del Spinner
            _spinner.Hexpand = true;
            _spinner.Vexpand = true;
            _spinner.Halign = Align.Center;
            _spinner.Valign = Align.Center;
            var spinnerSize = (int)Math.Floor(pHeight * 0.4);
            _spinner.SetSizeRequest(spinnerSize, spinnerSize);
            _spinner.Start();

            // Agregar el mensaje y el contenedor de botones al diálogo
            this.ContentArea.Add(_message);
            this.ContentArea.Add(_image);
            this.ContentArea.Add(_spinner);

            // Set Styles
            this.StyleContext.AddClass(ConfrimationDialogCssClases.ConfrimationDialog);
            this.Resizable = false;

            _spinner.Show();
        }


        /// <summary>
        /// Esconde el spinner y muestra el contenido que se pasa como parámetro.
        /// </summary>
        /// <param name="pMessage"> Message to show </param>
        /// <param name="pImgPathArr"> Path array to image </param>
        public void ShowContent (string pMessage, string[]? pImgPathArr = null) {

            // Configuramos la imagen si se pasa una
            string basePath = "";
            string imgPath = "";
            bool isValidExtension = false;
            bool imageExists = false;
            _imagePathArr = pImgPathArr;
            _image.Destroy();
            if (_imagePathArr != null && _imagePathArr.Length > 0) {
                
                basePath = AppDomain.CurrentDomain.BaseDirectory;
                imgPath = IO.Path.Combine(_imagePathArr.Prepend(basePath).ToArray());
            }

            if (basePath != imgPath) {
                imageExists = IO.File.Exists(imgPath);
                string imagePattern = @"\.(png|jpg|jpeg|bmp|gif|svg)$";
                isValidExtension = Regex.IsMatch(imgPath, imagePattern, RegexOptions.IgnoreCase);
            }

            if (imageExists && isValidExtension && imgPath.EndsWith(".gif"))
            {
                
                PixbufAnimation animation = new PixbufAnimation(imgPath);
                _image = new Image(animation);
            }
            else if (imageExists && isValidExtension){
                _image = new Image(imgPath);
            }

            _image.Vexpand = true;
            _image.Valign = Align.End;

            _message.Text = pMessage;
            _spinner.Hide();
            this.ContentArea.Add(_image);
            this.ContentArea.ReorderChild(_image, 0);
            _message.Show();
            _image.Show();

        }

    }
}   