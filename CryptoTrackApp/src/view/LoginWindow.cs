using System;
using Gtk;
using Gdk;
using UI = Gtk.Builder.ObjectAttribute;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CryptoTrackApp.src.services;
using CryptoTrackApp.src.view_managment;

namespace CryptoTrackApp.src.view
{
    public class LoginView : View
    {
        [UI] private Entry _emailInput;
        [UI] private Label _emailProblemLabel;
        [UI] private Label _passProblemLabel;
        [UI] private Entry _passwordInput;
        [UI] private EventBox _passVisibilityButton;
        [UI] private Image _imageButton;
        [UI] private Image _logo;
        [UI] private Button _loginButton;
        [UI] private Button _signInButton;
        [UI] private ButtonBox _buttonsBox;
        [UI] private Spinner _spinner;
        [UI] private Label _loginProblem;

        private bool isEmailValid = false;
        private bool isPasswordValid = true;

        private IUserServices? _userServices = null;
	
        private readonly string LOGO_PATH = "./src/assets/images/ctapp_logo.png";
        private readonly string BUTTON_IMAGE_OPEN_PATH = "./src/assets/images/toggle_visibility_white_open.ico";
        private readonly string BUTTON_IMAGE_CLOSE_PATH = "./src/assets/images/toggle_visibility_white_close.ico";


	public LoginView(IUserServices pUserServices) : base("LoginWindow") 
	{
	    this.CSS_PATH_DARK = "./src/css/login_window.css";
	    this.CSS_PATH_LIGHT = "";
	    this.SetStyle("dark");
	    _emailProblemLabel.Text = " ";
        _passProblemLabel.Text = " ";
        _loginProblem.Text = " ";
        this._spinner.Hide();
        this._buttonsBox.Homogeneous = true;
	    this.UserServices = pUserServices;
	}

	
	public IUserServices? UserServices {get { return this._userServices; } set { this._userServices = value; }}
	
        public override void ConfigEventHandlers()
        {
          _passVisibilityButton.ButtonReleaseEvent += PassVisibilityChanged;
          _loginButton.ButtonReleaseEvent += _LoginUserEvent;
          _signInButton.ButtonReleaseEvent += _SignUpEvent;
          _emailInput.Changed += EmailCheck;
        }

        public override void ConfigImages()
        {
            _imageButton.File = this.BUTTON_IMAGE_OPEN_PATH;
            _logo.File = this.LOGO_PATH;
        }

        private void PassVisibilityChanged(object sender, ButtonReleaseEventArgs a)
        {

            if (_passwordInput.Visibility)
            {
                _passwordInput.Visibility = false;
                _imageButton.File = this.BUTTON_IMAGE_OPEN_PATH;
            }
            else
            {
                _passwordInput.Visibility = true;
                _imageButton.File = this.BUTTON_IMAGE_CLOSE_PATH;
            }
        }

        private void EmailCheck(object sender, EventArgs e)
        {

            string patron = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

            Entry entry = (Entry)sender;
            string texto = entry.Text;


            if (!Regex.IsMatch(texto, patron))
            {
                _emailProblemLabel.Text = "Please enter a valid email.";
                       this.isEmailValid = false;
            }
            else
            {
                _emailProblemLabel.Text = " ";
                       this.isEmailValid = true;
            }
        }

        private async void _LoginUserEvent(object sender, ButtonReleaseEventArgs a)
        {

          if (isEmailValid && isPasswordValid)
          {

            this._loginProblem.Text = " ";
            this._loginButton.Visible = false;
            this._signInButton.Visible = false;
            this._buttonsBox.Hide();
            this._spinner.Show();
            bool res = await this._LoginUser(this.UserServices);
            this._spinner.Hide();
            this._buttonsBox.Show();
            this._loginButton.Visible = true;
            this._signInButton.Visible = true;

            if (res)
            {
                Console.WriteLine("Login...");
            }

          }
          else
          {
            this._loginProblem.Text = "* Please introduce a valid login data.";
          }

        }

        private async Task<bool> _LoginUser(IUserServices pUserServices)
        {
          AppResponse response = await pUserServices.LoginUser(this._passwordInput.Text, this._emailInput.Text);
          System.Console.WriteLine(response.message);
          if (response.status == "Failure")
          {
            this._loginProblem.Text = "* " + response.message;
            return false;
          }
            return true;
        }

        private void _SignUpEvent(object sender, ButtonReleaseEventArgs a)
        {
          IViewManager vw = ViewManager.GetInstance();
          vw.ChangeView("SignUp", this);
        }

    }

}
