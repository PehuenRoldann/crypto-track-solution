using System;
using Gtk;
using CryptoTrackApp.src.view.Windows;
using CryptoTrackApp.src.services;
using MessageDialog = CryptoTrackApp.src.view.Windows.MessageDialog;
using Gdk;

namespace CryptoTrackApp.src.view.Controllers
{

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

    /// <summary>
    /// Changes the actual view for other.
    /// </summary>
    /// <param name="pNewViewClass">New view class</param>
    /// <param name="pOldView">Old view (will be destroyed)</param>
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
      win.SetDefaultSize(900, 500);
      win.Move(0,0);
      win.Maximize();
      win.Show();
      
      if (pOldView != null) {this.App.RemoveWindow(pOldView); pOldView.Destroy(); }
    }

    public void ShowMainView(View pParent, string pUserId)
    {
      View win = new MainView(pUserId, new SubscriptionServices(), new CurrencyServices());
      this.App.AddWindow(win);
      win.SetDefaultSize(900, 500);
      win.Move(0,0);
      if (pParent.IsMaximized) {win.Maximize();}
      win.Show();
      pParent.Destroy();
    }

    public MessageDialog GetMessageDialog(
      View pParent, string pTitle, string pMessage,
      Image pImage, string pButtonLabel, int pWidth = 400, int pHeight = 300) {
      
      return new MessageDialog(pParent, pTitle, pMessage, pImage, pButtonLabel);

    }
  }
}
