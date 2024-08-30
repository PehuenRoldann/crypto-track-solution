using System;
using Gtk;
using Gdk;
using UI = Gtk.Builder.ObjectAttribute;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CryptoTrackApp.src.services;
using CryptoTrackApp.src.view.Controllers;

namespace CryptoTrackApp.src.view.Windows
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
	
        private readonly string LOGO_PATH = "./src/assets/images/logo_226x226.png";
        private readonly string BUTTON_IMAGE_OPEN_PATH = "./src/assets/images/toggle_visibility_white_open.ico";
        private readonly string BUTTON_IMAGE_CLOSE_PATH = "./src/assets/images/toggle_visibility_white_close.ico";


	public LoginView(IUserServices pUserServices) : base("LoginWindow") 
	{
      this._loginButton!.StyleContext.AddClass("button-color");
/* 	    this.CSS_PATH_DARK = "./src/css/login_window.css";
	    this.CSS_PATH_LIGHT = ""; */
	    this.SetStyle("./src/css/login_window.css");
	    _emailProblemLabel!.Text = " ";
      _passProblemLabel!.Text = " ";
      _loginProblem!.Text = " ";
      this._spinner!.Hide();
      this._buttonsBox!.Homogeneous = true;
	    this.UserServices = pUserServices;
	}

	
	public IUserServices? UserServices {get { return this._userServices; } set { this._userServices = value; }}
	
        public override void ConfigEventHandlers()
        {
          _passVisibilityButton.ButtonReleaseEvent += PassVisibilityChanged;
          _loginButton.ButtonReleaseEvent += _LoginUserEvent;
          _loginButton.Clicked += _LoginUserEvent!;
          this.KeyReleaseEvent += OnKeyReleaseEvent;
          _signInButton.ButtonReleaseEvent += _SignUpEvent;
          _emailInput.Changed += EmailCheck!;
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

         protected void OnKeyReleaseEvent(object sender, KeyReleaseEventArgs args)
        {
            // Verificar si la tecla presionada es Enter
            if (args.Event.Key == Gdk.Key.Return || args.Event.Key == Gdk.Key.KP_Enter)
            {
                this._loginButton.Activate(); // Activa el botón programáticamente
            }
        }

        // private void OnClickedLoginButton (object sender, Button)

        private async void _LoginUserEvent(object sender, EventArgs a)
        {

          if (isEmailValid && isPasswordValid)
          {

            this._loginProblem.Text = " ";
            this._loginButton.Visible = false;
            this._signInButton.Visible = false;
            this._buttonsBox.Hide();
            this._spinner.Show();

            string? res = await Task<string?>.Run( async () => {
              return await this._LoginUser(this.UserServices!);
            });

            this._spinner.Hide();
            this._buttonsBox.Show();
            this._loginButton.Visible = true;
            this._signInButton.Visible = true;

            if (res != null)
            {
                Console.WriteLine("Login...");
                var vw = ViewManager.GetInstance();
                vw.UserId = res;
                vw.ShowView("main", this);
            }

          }
          else
          {
            this._loginProblem.Text = "* Please introduce a valid login data.";
          }

        }

        private async Task<string?> _LoginUser(IUserServices pUserServices)
        {
          try
          {
            string? response = await pUserServices.LoginUser(this._passwordInput.Text, this._emailInput.Text);

            if (response == null)
            {
            this._loginProblem.Text = "* The proportioned email is not registered";
            }

            return response;
          }
          catch (InvalidOperationException)
          {
            this._loginProblem.Text = "* The proportioned password is wrong!";
          }
          catch (Exception)
          {
            this._loginProblem.Text = "* There has been an unknown problem while login, please try again."
                                      +"\nIf the problem persist, contact the support.";
          }
          
          return null;
        }

        private void _SignUpEvent(object sender, ButtonReleaseEventArgs a)
        {
          ViewManager vw = ViewManager.GetInstance();
          vw.ShowView("signup", this);
        }

    }

}
