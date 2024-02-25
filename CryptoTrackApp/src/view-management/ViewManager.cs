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

    public void ChangeView(string pNewViewClass, View? pOldView=null)
    {

      View? win = null;
      switch (pNewViewClass.ToLower()) {
	
	case "signup":
	  win = new SignUpView(new UserServices());
	  break;
	case "login":
	  win = new LoginView(new UserServices());
	  break;
	default:
	  throw new Exception("The parameter passed as viewClass is not valid.");
	  
      }

      if (pOldView != null) {

	if (pOldView.IsMaximized) {win.Maximize();}
	win.SetStyle(pOldView.CurrentStyle);
      }
      this.App.AddWindow(win);
      win.Show();
      
      if (pOldView != null) {this.App.RemoveWindow(pOldView); pOldView.Destroy(); }
    }

    //public Dialog ShowDateSelector () {
      
      //var dialog = new DateSelectionDialog();
      
      //return dialog;
    //}

    //public Dialog? GetDialog(string pDialogClass) {

      //Dialog? dialog;

      //switch (pDialogClass.ToLower()) {

	//case "dateselectiondialog":
	  //Console.WriteLine("Creando dialog");
	  //dialog = new DateSelectionDialog();
	  //break;
        //default:
	  //dialog =  null;
	  //break;
      //}

      //return dialog;
    //}

  }
}
