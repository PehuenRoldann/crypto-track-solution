using System;
using Gtk;
using Gdk;
using UI = Gtk.Builder.ObjectAttribute;
using System.Text.RegularExpressions;
using CryptoTrackApp.src.services;

namespace CryptoTrackApp.src.view
{
    public class LoginWindow : Gtk.Window
    {
        [UI] private Entry _emailInput = null;
        [UI] private Label _emailProblemLabel = null;
        [UI] private Label _passProblemLabel = null;
        [UI] private Entry _passwordInput = null;
        [UI] private EventBox _passVisibilityButton = null;
        [UI] private Image _imageButton = null;
        [UI] private Image _logo = null;
        [UI] private Button _loginButton = null;
        [UI] private Button _signInButton = null;
        private IUserServices userServices = new UserServices();

        private CssProvider cssProvider = new CssProvider();

        public LoginWindow() : this(new Builder("LoginWindow.glade")) { }

        private LoginWindow(Builder builder) : base(builder.GetRawOwnedObject("LoginWindow"))
        {
            
            builder.Autoconnect(this);
            DeleteEvent += Window_DeleteEvent;
            this._ConfigButtons();
            this._ConfigInputs();
            this._ImagesConfig();
            this._ConfigStyles();
            _emailProblemLabel.Text = " ";
            _passProblemLabel.Text = " ";
            
        }

        private void _ConfigStyles() {
            
            cssProvider.LoadFromPath("CryptoTrackApp/src/css/login_window.css");
            StyleContext.AddProviderForScreen(Gdk.Screen.Default, cssProvider, 800);
            _loginButton.StyleContext.AddClass("button-color");
            _signInButton.StyleContext.AddClass("sign-in-button");
            _passVisibilityButton.StyleContext.AddClass("button-image");
	    _logo.StyleContext.AddClass("logo-image");
        }
        private void _ConfigButtons () {
            _passVisibilityButton.ButtonReleaseEvent += PassVisibilityChanged;
            _loginButton.ButtonReleaseEvent += _LoginUserEvent;
	    _signInButton.ButtonReleaseEvent += _SignUpEvent;
        }

        private void _ConfigInputs () {
            _emailInput.Changed += EmailCheck;
            
        }
        private void _ImagesConfig () {
            _imageButton.File = "CryptoTrackApp/src/assets/images/toggle_visibility_white_open.ico";
            _logo.File = "CryptoTrackApp/src/assets/images/ctapp_logo.png";
        }

        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }

        private void PassVisibilityChanged(object sender, ButtonReleaseEventArgs a) {
            
            if (_passwordInput.Visibility) {
                _passwordInput.Visibility = false;
                _imageButton.File = "src/assets/images/toggle_visibility_white_open.ico";
                }
            else {
                _passwordInput.Visibility = true;
                _imageButton.File = "src/assets/images/toggle_visibility_white_close.ico";
                }
        }

        private void EmailCheck (object sender, EventArgs e) {

            string patron = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

            Entry entry = (Entry)sender;
            string texto = entry.Text;


            if (!Regex.IsMatch(texto, patron)){
                _emailProblemLabel.Text = "Please enter a valid email.";
            }
            else {
                _emailProblemLabel.Text = " ";
            }
        }

        private void _LoginUserEvent(object sender, ButtonReleaseEventArgs a) {

            this._LoginUser(this.userServices);
        }

        private void _LoginUser(IUserServices pUserServices) {
            AppResponse response = pUserServices.LoginUser(this._passwordInput.Text,
                                                            this._emailInput.Text);
            System.Console.WriteLine(response.message);
        }

	private void _SignUpEvent(object sender, ButtonReleaseEventArgs a) {
	  var signUpWindow = new SignUpWindow(); 
	  signUpWindow.Show();
	  this.Destroy();
	}

    }

}
