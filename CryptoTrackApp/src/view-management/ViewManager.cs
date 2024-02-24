using System;
using Gtk;
using CryptoTrackApp.src.view;
using CryptoTrackApp.src.services;

namespace CryptoTrackApp.src.view_managment {

  public class ViewManager : IViewManager {
    
    private Application? _app;

    private static ViewManager _instance;

    private static readonly object _lock = new Object();
    
    private ViewManager() {}

    public static ViewManager GetInstance()
    {
      if (_instance == null)
      {
	lock (_lock)
	{
	  if (_instance == null)
	  {
	    _instance = new ViewManager();
	  }
	}
      }

      return _instance;
    }

    public Application? App {get {return this._app; } set {this._app = value; }}

    public void ChangeView(string viewClass, View? oldView=null)
    {

      View? win = null;
      switch (viewClass) {
	
	case "SignUp":
	  win = new SignUpView(new UserServices());
	  break;
	case "Login":
	  win = new LoginView(new UserServices());
	  break;
	default:
	  throw new Exception("The parameter passed as viewClass is not valid.");
	  
      }

      if (oldView != null) {

	if (oldView.IsMaximized) {win.Maximize();}
	win.SetStyle(oldView.CurrentStyle);
      }
      this.App.AddWindow(win);
      win.Show();
      
      if (oldView != null) {this.App.RemoveWindow(oldView); oldView.Destroy(); }
    }

  }
}
