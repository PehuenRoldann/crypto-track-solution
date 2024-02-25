using System;
using System.Globalization;
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
    private bool isPasswordConfirmed = false;
// ---- CONSTRUCTOR ------------------------------------------------------------
    public SignUpView(IUserServices pUserServices) : base("SignUpWindow") {
      this.CSS_PATH_DARK = "./src/css/SignUpWindow.css";
      this.CSS_PATH_LIGHT = "./src/css/SignUpWindow.css";
      this.userServices = pUserServices;
      this.SetStyle("dark");
      this.CheckSignUpButton();
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
      this._confEmailEntry.Changed += ConfirmMailCheck;
      this._passwordEntry.Changed += PasswordCheck;
      this._passwordEntry.Changed += ConfirmPasswordCheck;
      this._confPasswordEntry.Changed += ConfirmPasswordCheck;
      this._birthDateEntry.FocusGrabbed += ShowDateSelector;
    }
// ---- EVENTS HANDLERS --------------------------------------------
    private void CancelButtonReleased(object sender, ButtonReleaseEventArgs a) {
     // Return to the Login view, closing this window. 
      IViewManager vw = ViewManager.GetInstance();
      vw.ChangeView("Login", this);
    }


    private void ShowDateSelector(object sender, EventArgs a) {
    // Creates a Dialog with a Gtk.Calendar to select the birth date.

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
	};

        dialog.ContentArea.Add(calendar);
	dialog.ContentArea.Add(calendarButton);

        dialog.ShowAll();

	//a.RetVal = true;
    }
// ---- VALIDATORS ---------------------------------------------------------------
    private void CheckSignUpButton(){
    // If all the validators are TRUE, the SignUp button can listen to events.
    // If one validator is FALSE, disable the SignUp button and change its style.
      if (this.isEmailValid && this.isConfEmailValid && this.isPasswordValid && this.isPasswordConfirmed) {
	this._signUpButton.Sensitive = true;
	this._signUpButton.StyleContext.RemoveClass("sign-up-button-disable");
      }
      else {
	this._signUpButton.Sensitive = false;
	this._signUpButton.StyleContext.AddClass("sign-up-button-disable");
      }
    }

    private async void EmailCheck(object sender, EventArgs e)
    {
      string patron = @"^(?!$)[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9.]+$";
      Entry entry = (Entry)sender;
      string email = entry.Text;
      
            
      if (!Regex.IsMatch(email, patron) || email == "")
      {
	entry.SetIconFromPixbuf(EntryIconPosition.Secondary, IconManager.invalid_icon);
	entry.SecondaryIconTooltipText = "The email format is invalid, please intreduce a valid\nemail format, ej: your_email@address.com";
        this.isEmailValid = false;
      }
      else {
	try{
	    bool available = await Task.Run( () => userServices.IsEmailAvailable(email));
	    if (!available)
	    {
	      Application.Invoke((sender, args) => 
		{
		  entry.SetIconFromPixbuf(EntryIconPosition.Secondary, IconManager.invalid_icon);
		  entry.SecondaryIconTooltipText = "This email is already registered!";
		  this.isEmailValid = false;
		});
	    }
	    else
	    {
	      Application.Invoke((sender, args) => 
		{
		  entry.SetIconFromPixbuf(EntryIconPosition.Secondary, IconManager.valid_icon);
		  entry.SecondaryIconTooltipText = "This email is valid.";
		  this.isEmailValid = true;
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
	this._confEmailEntry.SetIconFromPixbuf(EntryIconPosition.Secondary, IconManager.invalid_icon);
	this._confEmailEntry.SecondaryIconTooltipText = "The emails must be the same!";
	this.isConfEmailValid = false;
      } 
      else 
      {
	this._confEmailEntry.SetIconFromPixbuf(EntryIconPosition.Secondary, IconManager.valid_icon);
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
	this._passwordEntry.SetIconFromPixbuf(EntryIconPosition.Secondary, IconManager.valid_icon);
      }
      else {
	this.isPasswordValid = false;
	this._passwordEntry.SetIconFromPixbuf(EntryIconPosition.Secondary, IconManager.invalid_icon);
	this._passwordEntry.SecondaryIconTooltipText = "Passwords must have:\n- 1 Uppercase character.\n- 1 Digit.\n- 1 Special character.\n- 6 Characters at least.";
      }

      this.CheckSignUpButton();
    }
    private void ConfirmPasswordCheck (object sender, EventArgs e) {
      string input = this._confPasswordEntry.Text;
      string pass = this._passwordEntry.Text;

      if (input != pass) {
	this._confPasswordEntry.SetIconFromPixbuf(EntryIconPosition.Secondary, IconManager.invalid_icon);
	this._confPasswordEntry.SecondaryIconTooltipText = "Passwords should be the same!";
	this.isPasswordConfirmed = false;
      }
      else {
	this._confPasswordEntry.SetIconFromPixbuf(EntryIconPosition.Secondary, IconManager.valid_icon);
	this._confPasswordEntry.SecondaryIconTooltipText = "Passwords are equal.";
	this.isPasswordConfirmed = true;
      }
    }

  
  }

}
