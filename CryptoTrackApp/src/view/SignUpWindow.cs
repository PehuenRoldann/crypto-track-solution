using System;
using Gtk;
using Gdk;
using UI = Gtk.Builder.ObjectAttribute;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CryptoTrackApp.src.services;
using CryptoTrackApp.src.view_managment;

namespace CryptoTrackApp.src.view {
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
//    [UI] Label? _emailProblemLabel = null;
    private IUserServices userServices;

    private readonly string CSS_PATH = "./src/css/SignUpWindow.css";
    private readonly string LOGO_PATH = "./src/assets/images/ctapp_logo.png";
    private CssProvider cssProvider = new CssProvider();
    
// ---- VALIDATORS FIELDS ------------------------------------------------------
    private bool isEmailValid = false;
    private bool isConfEmailValid = false;
    private bool isPasswordValid = false;
// ---- CONSTRUCTOR ------------------------------------------------------------
    public SignUpView(IUserServices pUserServices) : base("SignUpWindow") {
      this.CSS_PATH_DARK = "./src/css/SignUpWindow.css";
      this.CSS_PATH_LIGHT = "./src/css/SignUpWindow.css";
      this.userServices = pUserServices;
      this.SetStyle("dark");
      this.CheckSignUpButton();
      //this._ConfigInputs();
      //this.ConfigImages();
      this.ConfigHandlers();
    }

// ---- INITIAL CONFIGURATIONS --------------------------------------------------
    public override void ConfigImages(){
      _logo.File = this.LOGO_PATH;
    }
    
    public override void ConfigButtons() {
      this._cancelButton.ButtonReleaseEvent += CancelButtonReleased;
    }
    
    private void ConfigHandlers () {
      this._emailEntry.Changed += EmailCheck;
      this._emailEntry.Changed += ConfirmMailCheck;
      this._emailEntry.FocusOutEvent += EmailCheckAvailable;
      this._confEmailEntry.Changed += ConfirmMailCheck;
      this._passwordEntry.Changed += PasswordCheck;
    }
// ---- EVENTS HANDLERS --------------------------------------------
    private void CancelButtonReleased(object sender, ButtonReleaseEventArgs a) {
      
      IViewManager vw = ViewManager.GetInstance();
      vw.ChangeView("Login", this);
    }
    //private void _SignUpEvent() {

    //}
// ---- VALIDATORS ---------------------------------------------------------------
    private void CheckSignUpButton(){
      if (this.isEmailValid && this.isConfEmailValid && this.isPasswordValid) {
	this._signUpButton.Sensitive = true;
	this._signUpButton.StyleContext.RemoveClass("sign-up-button-disable");
      }
      else {
	this._signUpButton.Sensitive = false;
	this._signUpButton.StyleContext.AddClass("sign-up-button-disable");
      }
    }

    private void EmailCheck(object sender, EventArgs e)
    {
      string patron = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
      Entry entry = (Entry)sender;
      string email = entry.Text;
      
            
      if (!Regex.IsMatch(email, patron))
      {
	//_emailEntry.IconP
	entry.SetIconFromIconName(EntryIconPosition.Secondary, "dialog-error");
	entry.SecondaryIconTooltipText = "The email format is invalid, please intreduce a valid\nemail format, ej: your_email@address.com";
        this.isEmailValid = false;
      }
      else
      {
	entry.SetIconFromIconName(EntryIconPosition.Secondary, "emblem-default");
	entry.SecondaryIconTooltipText = "This email is valid!";
       //_emailProblemLabel.Text = " ";
        this.isEmailValid = true;
      }

      this.CheckSignUpButton();
    }

    private async void EmailCheckAvailable(object sender, FocusOutEventArgs a)
    {
      if (this.isEmailValid) 
      {
	Entry entry = (Entry)sender;
	string email = entry.Text;
	try{
	  bool available = await Task.Run( () => userServices.IsEmailAvailable(email));
	  if (!available)
	  {
	    Application.Invoke((sender, args) => 
		{
		  entry.SetIconFromIconName(EntryIconPosition.Secondary, "dialog-error");
		  entry.SecondaryIconTooltipText = "This email is already registered!";
		  this.isEmailValid = false;
		});
	  }
	}
	catch (Exception error) {
	  Console.WriteLine("Error at CryptoTrackApp.src.view.SignUpView.EmailCheckAvailable: " + error.Message);
	}
      }

      this.CheckSignUpButton();
    }

    private void ConfirmMailCheck(object sender, EventArgs e)
    {
      if (this._emailEntry.Text != this._confEmailEntry.Text)
      {
	this._confEmailEntry.SetIconFromIconName(EntryIconPosition.Secondary, "dialog-error");
	this._confEmailEntry.SecondaryIconTooltipText = "The emails must be the same!";
	this.isConfEmailValid = false;
      } 
      else 
      {
	this._confEmailEntry.SetIconFromIconName(EntryIconPosition.Secondary, "emblem-default");
	this._confEmailEntry.SecondaryIconTooltipText = "The emails are the same.";
	this.isConfEmailValid = true;
      }

      this.CheckSignUpButton();
    }

    private void PasswordCheck(object sender, EventArgs e) {

      string pass = this._passwordEntry.Text;
      bool atLeast = pass.Length >= 6;
      bool haveNumber = Regex.IsMatch(pass, @"\d");
      bool haveMayus = Regex.IsMatch(pass, "[A-Z]");
      bool haveSpecial = Regex.IsMatch(pass, @"[!@#$%^&*()_+{}\[\]:;<>,.?~\\-]");

      if (haveMayus && haveNumber && haveSpecial && atLeast){
	this.isPasswordValid = true;
	this._passwordEntry.SetIconFromIconName(EntryIconPosition.Secondary, "emblem-default");
      }
      else {
	this.isPasswordValid = false;
	this._passwordEntry.SetIconFromIconName(EntryIconPosition.Secondary, "dialog-error");
	this._passwordEntry.SecondaryIconTooltipText = "Passwords must have:\n- 1 Uppercase character.\n- 1 Digit.\n- 1 Special character.\n- 6 Characters at least.";
      }

      this.CheckSignUpButton();
    }
  }
}
