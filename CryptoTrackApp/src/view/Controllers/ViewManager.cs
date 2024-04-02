using System;
using Gtk;
using CryptoTrackApp.src.view.Windows;
using CryptoTrackApp.src.services;
using MessageDialog = CryptoTrackApp.src.view.Windows.MessageDialog;
using Gdk;

namespace CryptoTrackApp.src.view.Controllers
{

  public class ViewManager : IViewManager
  {
    public string UserId {get; set;}

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


    public void ShowView(string pViewType, View? pParent = null)
    {
      View win;
      switch (pViewType.ToLower())
      {
        case "login":
          win = new LoginView(new UserServices());
          break;
        
        case "signup":
          win = new SignUpView(new UserServices());
          break;
        
        case "main":
          win = new MainView(this.UserId, new SubscriptionServices(), new CurrencyServices());
          break;
        
        case "follow":
          win = new FollowView(new SubscriptionServices(), new CurrencyServices());
          break;
        
        default:
          win = new LoginView(new UserServices());
          break;
      }

      this.InitView(win, pParent);
    }

    private void InitView(View win, View? pParent)
    {
      this.App.AddWindow(win);
      win.SetDefaultSize(1280, 720);
      /* win.Move(0,0); */
      if (pParent != null && pParent.IsMaximized) {win.Maximize();}
      win.Show();
      if (pParent != null) {this.App.RemoveWindow(pParent); pParent.Destroy();}
    }

    public MessageDialog GetMessageDialog(View pParent, string pTitle, string pMessage,Image pImage,
        string pButtonLabel, int pWidth = 400, int pHeight = 300)
    { 
      return new MessageDialog(pParent, pTitle, pMessage, pImage, pButtonLabel);
    }
  }
}
