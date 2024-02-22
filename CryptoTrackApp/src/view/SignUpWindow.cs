using System;
using Gtk;
using Gdk;
using UI = Gtk.Builder.ObjectAttribute;
using System.Text.RegularExpressions;
using CryptoTrackApp.src.services;

namespace CryptoTrackApp.src.view {
  public class SignUpWindow : Gtk.Window 
  {
    public SignUpWindow() : this(new Builder("SignUpWindow.glade")) { }

    private SignUpWindow(Builder builder) : base(builder.GetRawOwnedObject("SignUpWindow"))
    {
     
      builder.Autoconnect(this);
      DeleteEvent += Window_DeleteEvent;
      //this._ConfigButtons();
      //this._ConfigInputs();
      //this._ImagesConfig();
      //this._ConfigStyles();
      //_emailProblemLabel.Text = " ";
      //_passProblemLabel.Text = " ";
            
    }
    
    private void Window_DeleteEvent(object sender, DeleteEventArgs a)
    {
      Application.Quit();
    }
  }
}
