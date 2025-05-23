using System;
using System.Globalization;
using Gtk;
using Gdk;
using UI = Gtk.Builder.ObjectAttribute;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CryptoTrackApp.src.services;
using CryptoTrackApp.src.utils;
using CryptoTrackApp.src.view.helpers;
using COMP = CryptoTrackApp.src.view.components;
using Npgsql.Replication;
/* using GLib; */

namespace CryptoTrackApp.src.view.windows {
    public class SignUpView : View
    {
        [UI] Image? _logo = null;
        [UI] Entry? _userNameEntry = null;
        [UI] Entry? _emailEntry = null;
        [UI] Entry? _confEmailEntry = null;
        [UI] Entry? _passwordEntry = null;
        [UI] Entry? _confPasswordEntry = null;
        [UI] Entry? _birthDateEntry = null;
        [UI] Button? _signUpButton = null;
        [UI] Button? _cancelButton = null;
        [UI] Spinner? _spinner = null;
        private IUserServices userServices;

        private Logger _logger = new Logger();

        private readonly string CSS_PATH = PathFinder.GetPath(CssFilesPaths.SignUpWindowCss);
        private readonly string LOGO_PATH = PathFinder.GetPath(ImagesArrPaths.AppLogox266);
        
    // ---- VALIDATORS FIELDS ------------------------------------------------------
        private bool isEmailValid = false;
        private bool isConfEmailValid = false;
        private bool isPasswordValid = false;
        private bool isPasswordConfirmed = false;
    // ---- CONSTRUCTOR ------------------------------------------------------------
        public SignUpView(IUserServices pUserServices) : base(Templates.SignUpWindow) {
        this.userServices = pUserServices;
        this.SetStyle(CssFilesPaths.SignUpWindowCss);
        this.CheckSignUpButton();
        this._spinner.Hide();

        }

    // ---- INITIAL CONFIGURATIONS --------------------------------------------------
        public override void ConfigImages(){
        _logo.File = this.LOGO_PATH;
        }
            
        
        public override void ConfigEventHandlers () {

            this._cancelButton.ButtonReleaseEvent += CancelButtonReleased;
            this._emailEntry.Changed += EmailCheck;
            this._emailEntry.Changed += ConfirmMailCheck;
            this._confEmailEntry.Changed += ConfirmMailCheck;
            this._passwordEntry.Changed += PasswordCheck;
            this._passwordEntry.Changed += ConfirmPasswordCheck;
            this._confPasswordEntry.Changed += ConfirmPasswordCheck;
            this._birthDateEntry.FocusGrabbed += ShowDateSelector;
            this._signUpButton.ButtonReleaseEvent += SignUpPressed;
        }

    // ---- EVENTS HANDLERS --------------------------------------------
        /// <summary>
        /// Returns to login view, closing this window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="a"></param>
        private void CancelButtonReleased(object sender, ButtonReleaseEventArgs a) {
            ViewManager vw = ViewManager.GetInstance();
            vw.ShowView("login", this);
        }

        /// <summary>
        /// Crates a dialog with Gtk.Calendar to select the birth date.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="a"></param>
        private void ShowDateSelector(object sender, EventArgs a) {

            Dialog dialog = new Dialog("Selector de Fecha", this, DialogFlags.Modal, ButtonsType.OkCancel);
            DateTime currentDate = new DateTime();
            if (this._birthDateEntry.Text != "") {
            currentDate =  DateTime.ParseExact(this._birthDateEntry.Text, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }
            else {
            currentDate =  DateTime.ParseExact(this._birthDateEntry.PlaceholderText, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }

            Gtk.Calendar calendar = new Gtk.Calendar();
            calendar.Year = currentDate.Year;
            calendar.Month = currentDate.Month;
            calendar.Day = currentDate.Day;
            Button calendarButton = new Button();
            calendarButton.Label = "Confirm";

            calendarButton.ButtonReleaseEvent += (obj, ev) => {

            DateTime selectedDate = new DateTime (calendar.Year, calendar.Month + 1 , calendar.Day);
            ((Entry)sender).Text = selectedDate.ToString("dd-MM-yyyy");

            dialog.Hide();
            this.CheckSignUpButton();
            }; 
            dialog.ContentArea.Add(calendar);
            dialog.ContentArea.Add(calendarButton);

            dialog.ShowAll();
        }

        private async void SignUpPressed (object sender, ButtonReleaseEventArgs args)
        {


            this._signUpButton.Hide();
            this._spinner.Show();

            /* Dialog dialog = ViewManager.GetInstance().GetDialog(this, "Result"); */
            string message = "";
            Image image;
            string buttonText = "";
            var emailCheck = await this.CheckEmailAvailable();

            if (emailCheck == null) {
                message = "There has been a problem with the request, try again later.";
                image = new Image(IconManager.invalid_icon);
                buttonText= "Close";
            }
            else if (!emailCheck.Value) {
                message = "There is a user with this email already!\nPlease use other email.";
                image = new Image(IconManager.invalid_icon);
                buttonText= "Close";
            }
            else {

                try
                {
                    await Task.Run(() => {
                        return this.userServices.AddUserAsync(
                            this._emailEntry.Text,
                            this._passwordEntry.Text,
                            this._userNameEntry.Text,
                            DateTime.ParseExact(this._birthDateEntry.Text, "dd-MM-yyyy", CultureInfo.InstalledUICulture)
                        );
                    });

                    PixbufAnimation animation = new PixbufAnimation("src/assets/gifs/checkmark_light.gif");
                    image = new Image(animation);
                    message = "Usuario creado exitosamente!";
                    buttonText = "Login";

                }
                catch (Exception error)
                {
                    image = new Image(IconManager.invalid_icon);
                    message = $"Error: {error.Message}";
                    buttonText = "Colse";
                }

            }

            Dialog dialog = new COMP.MessageDialog(this, "Title", message, image, buttonText);
            
            if (buttonText == "Login")
            {
                dialog.ButtonReleaseEvent += (obj, ev) => {
                    dialog.Destroy();
                    ViewManager.GetInstance().ShowView(ViewsIds.Login, this);
                };
            }
            else
            {
                dialog.ButtonReleaseEvent += (obj, ev) => {
                    dialog.Destroy();
                    this._spinner.Hide();
                    this._signUpButton.ShowAll();
                };
            }

            dialog.ShowAll();

        }


    // ---- VALIDATORS ---------------------------------------------------------------
        /// <summary>
        /// If all validators are TRUE, the SignUp button can listen to evetns.<br>
        /// If one is FALSE, disable the SignUp button and change its style.
        /// </summary>
        private void CheckSignUpButton()
        {
            if (this.isEmailValid && this.isConfEmailValid && this.isPasswordValid 
            && this.isPasswordConfirmed && this._birthDateEntry.Text != "")
            {
                this._signUpButton.Sensitive = true;
                this._signUpButton.StyleContext.RemoveClass("sign-up-button-disable");
            }
            else {
                this._signUpButton.Sensitive = false;
                this._signUpButton.StyleContext.AddClass("sign-up-button-disable");
            }
        }


        /// <summary>
        /// Checks if the input email is valid.
        /// Changes this.isEmailValid property.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EmailCheck(object sender, EventArgs e)
        {

            this.isEmailValid = false;
            string patron = @"^(?!$)[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[com|ar|]+$";
            Entry entry = (Entry)sender;
            string email = entry.Text;
            
                    
            if (!Regex.IsMatch(email, patron) || email == "") {

                entry.SetIconFromPixbuf(EntryIconPosition.Secondary, IconManager.invalid_icon);
                entry.SecondaryIconTooltipText = "The email format is invalid, please intreduce a valid\nemail format, ej: your_email@address.com";
            }
            else {
                this.isEmailValid = true;
                entry.SetIconFromPixbuf(EntryIconPosition.Secondary, IconManager.valid_icon);
                entry.SecondaryIconTooltipText = "This email is valid.";
            }

        }


        ///<summary>
        ///Check if the email is available to register a new user.
        /// Changes this.isEmailValid property.
        ///</summary>
        ///<returns>
        ///True: if the email is available.
        ///False: if the email es not available.
        ///Null: if there was a problem with the query.
        ///</returns>
        private async Task<bool?> CheckEmailAvailable()
        {
            Entry entry = this._emailEntry;
            string email = entry.Text;
            bool? available;
            try
            {
                available = await Task.Run( () => userServices.IsEmailAvailable(email));
                if (available.HasValue && !available.Value)
                {
                    Application.Invoke((sender, args) => 
                    {
                        entry.SetIconFromPixbuf(EntryIconPosition.Secondary, IconManager.invalid_icon);
                        entry.SecondaryIconTooltipText = "This email is already registered!";
                        this.isEmailValid = false; 
                    });
                }
            }
            catch (Exception error)
            {
                _logger.Log($"[ERROR - CheckEmailAvailable at SignUpView - message: {error.Message}]");
                available = null;
            }
            finally {
                this.CheckSignUpButton();
            }

            return available;
        }



        /// <summary>
        /// Validate the email previously introduced by the user. <br>
        /// Chantes the this.isConfEmailValid property.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfirmMailCheck(object sender, EventArgs e)
        {

            this.isConfEmailValid = false;
            if (this._emailEntry.Text != this._confEmailEntry.Text)
            {
                this._confEmailEntry.SetIconFromPixbuf(EntryIconPosition.Secondary, IconManager.invalid_icon);
                this._confEmailEntry.SecondaryIconTooltipText = "The emails must be the same!";
            } 
            else
            {
                this._confEmailEntry.SetIconFromPixbuf(EntryIconPosition.Secondary, IconManager.valid_icon);
                this._confEmailEntry.SecondaryIconTooltipText = "The emails are the same.";
                this.isConfEmailValid = true;
            }

            this.CheckSignUpButton();
        }



        /// <summary>
        /// Check if the user's password meets the requirements.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param> <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PasswordCheck(object sender, EventArgs e)
        {

            this.isPasswordValid = false;
            string pass = this._passwordEntry.Text;
            bool atLeast = pass.Length >= 6;
            bool haveNumber = Regex.IsMatch(pass, @"\d");
            bool haveMayus = Regex.IsMatch(pass, "[A-Z]");
            bool haveSpecial = Regex.IsMatch(pass, @"[!@#$%^&*()_+{}\[\]:;<>,.?~\\-]");

            if (haveMayus && haveNumber && haveSpecial && atLeast)
            {
                this.isPasswordValid = true;
                this._passwordEntry.SetIconFromPixbuf(EntryIconPosition.Secondary, IconManager.valid_icon);
            }
            else
            {
                this._passwordEntry.SetIconFromPixbuf(EntryIconPosition.Secondary, IconManager.invalid_icon);
                this._passwordEntry.SecondaryIconTooltipText = "Passwords must have:\n- 1 Uppercase character.\n- 1 Digit.\n- 1 Special character.\n- 6 Characters at least.";
            }

            this.CheckSignUpButton();
        }



        /// <summary>
        /// Validates the password previously given by the user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfirmPasswordCheck(object sender, EventArgs e)
        {

            this.isPasswordConfirmed = false;
            string input = this._confPasswordEntry.Text;
            string pass = this._passwordEntry.Text;

            if (input != pass)
            {
                this._confPasswordEntry.SetIconFromPixbuf(EntryIconPosition.Secondary, IconManager.invalid_icon);
                this._confPasswordEntry.SecondaryIconTooltipText = "Passwords should be the same!";
            }
            else
            {
                this._confPasswordEntry.SetIconFromPixbuf(EntryIconPosition.Secondary, IconManager.valid_icon);
                this._confPasswordEntry.SecondaryIconTooltipText = "Passwords are equal.";
                this.isPasswordConfirmed = true;
            }

            this.CheckSignUpButton();
        }

    }

}
