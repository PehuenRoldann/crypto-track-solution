using System;
using Gtk;
using Gdk;
using UI = Gtk.Builder.ObjectAttribute;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
	[UI] private ButtonBox _buttonsBox = null;
	[UI] private Spinner _spinner = null;
	[UI] private Label _loginProblem = null;

	private bool isEmailValid = false;
	private bool isPasswordValid = true;

	CssProvider cssProvider = new CssProvider();
        private IUserServices userServices = new UserServices();

	private readonly string CSS_PATH = "./src/css/login_window.css";
	private readonly string LOGO_PATH = "./src/assets/images/ctapp_logo.png";
	private readonly string BUTTON_IMAGE_OPEN_PATH = "./src/assets/images/toggle_visibility_white_open.ico";
	private readonly string BUTTON_IMAGE_CLOSE_PATH = "./src/assets/images/toggle_visibility_white_close.ico";
	

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
	    _loginProblem.Text = " ";
	    this._spinner.Hide();
	    this._buttonsBox.Homogeneous = true;
            
        }

        private void _ConfigStyles() {
            
            cssProvider.LoadFromPath(this.CSS_PATH);
	    StyleContext.AddProviderForScreen(Gdk.Screen.Default, cssProvider, 1000);
	    //this.StyleContext.AddProvider(cssProvider, 800);
	    //_loginButton.StyleContext.AddClass("button-color");
	    //_signInButton.StyleContext.AddClass("sign-in-button");
	    //_passVisibilityButton.StyleContext.AddClass("button-image");
	    //_logo.StyleContext.AddClass("logo-image");
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
            _imageButton.File = this.BUTTON_IMAGE_OPEN_PATH;
            _logo.File = this.LOGO_PATH;
        }

        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }

        private void PassVisibilityChanged(object sender, ButtonReleaseEventArgs a) {
            
            if (_passwordInput.Visibility) {
                _passwordInput.Visibility = false;
                _imageButton.File = this.BUTTON_IMAGE_OPEN_PATH;
                }
            else {
                _passwordInput.Visibility = true;
                _imageButton.File = this.BUTTON_IMAGE_CLOSE_PATH;
                }
        }

        private void EmailCheck (object sender, EventArgs e) {

            string patron = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

            Entry entry = (Entry)sender;
            string texto = entry.Text;


            if (!Regex.IsMatch(texto, patron)){
                _emailProblemLabel.Text = "Please enter a valid email.";
		this.isEmailValid = false;
            }
            else {
                _emailProblemLabel.Text = " ";
		this.isEmailValid = true;
            }
        }

        private async void _LoginUserEvent(object sender, ButtonReleaseEventArgs a) {

	  if (isEmailValid && isPasswordValid)
	  {

	    this._loginProblem.Text = " ";
	    this._loginButton.Visible = false;
	    this._signInButton.Visible = false;
	    this._buttonsBox.Hide();
	    this._spinner.Show();
	    bool res = await this._LoginUser(this.userServices);
	    this._spinner.Hide();
	    this._buttonsBox.Show();
	    this._loginButton.Visible = true;
	    this._signInButton.Visible = true;

	    if (res) {
	      Console.WriteLine("Login...");
	    }

	  }
	  else 
	  {
	  this._loginProblem.Text = "* Please introduce a valid login data.";
	  }

        }

      private async Task<bool> _LoginUser(IUserServices pUserServices) {
          AppResponse response = await pUserServices.LoginUser(this._passwordInput.Text,this._emailInput.Text);
          System.Console.WriteLine(response.message);
	  
	  if (response.status == "Failure"){
	    this._loginProblem.Text = "* "+response.message;
	    return false;
	  }
	  return true;
	  
        }

	private void _SignUpEvent(object sender, ButtonReleaseEventArgs a) {
	  var signUpWindow = new SignUpWindow(); 
	  signUpWindow.Show();
	  this.Destroy();
	}

    }

}
