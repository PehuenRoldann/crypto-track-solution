using System;
using Gtk;
using Gdk;
using UI = Gtk.Builder.ObjectAttribute;
using System.Text.RegularExpressions;
using CryptoTrackApp.src.services;

namespace CryptoTrackApp.src.view {
  public class SignUpWindow : Gtk.Window 
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
    
    
    private readonly string CSS_PATH = "./src/css/SignUpWindow.css";
    private readonly string LOGO_PATH = "./src/assets/images/ctapp_logo.png";
    private CssProvider cssProvider = new CssProvider();

    public SignUpWindow() : this(new Builder("SignUpWindow.glade")) { }

    private SignUpWindow(Builder builder) : base(builder.GetRawOwnedObject("SignUpWindow"))
    {

      //if (Application.ActiveWindow.IsMaximized){

	//this.Maximize();

      //}
     
      builder.Autoconnect(this);
      DeleteEvent += Window_DeleteEvent;
      //this._ConfigButtons();
      //this._ConfigInputs();
      this._ConfigImages();
      this._ConfigStyles();
      //_emailProblemLabel.Text = " ";
      //_passProblemLabel.Text = " ";
            
    }
    
    private void Window_DeleteEvent(object sender, DeleteEventArgs a)
    {
      Application.Quit();
    }

    private void _ConfigStyles() {
      cssProvider.LoadFromPath(this.CSS_PATH);
      //StyleContext.AddProviderForScreen(Gdk.Screen.Default, cssProvider, 1000);
      StyleContext.AddProviderForScreen(Gdk.Screen.Default ,cssProvider, 1000);
      
      //_loginButton.StyleContext.AddClass("button-color");
      //_signInButton.StyleContext.AddClass("sign-in-button");
      //_passVisibilityButton.StyleContext.AddClass("button-image");
      //_logo.StyleContext.AddClass("logo-image");
    }

    private void _ConfigImages(){
      _logo.File = this.LOGO_PATH;
    }

    //private void _SignUpEvent() {

    //}

  }
}
