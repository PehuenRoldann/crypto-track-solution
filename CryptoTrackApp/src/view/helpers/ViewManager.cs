using System;
using Gtk;
using CryptoTrackApp.src.view.windows;
using CryptoTrackApp.src.services;
using MessageDialog = CryptoTrackApp.src.view.components.MessageDialog;
using Gdk;
using CryptoTrackApp.src.utils;


namespace CryptoTrackApp.src.view.helpers
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
        string viewTypeKey = pViewType.ToLower();

        View win;
        switch (viewTypeKey)
        {
            case ViewsIds.Login:
                win = new LoginView(new UserServices());
                break;

            case ViewsIds.SignUp:
                win = new SignUpView(new UserServices());
                break;

            case ViewsIds.Main:
                win = new MainView(this.UserId, new SubscriptionServices(),
                    new CurrencyServices(), new PlotterService());
                break;

            case ViewsIds.Follow:
                win = new FollowView(new SubscriptionServices(), new CurrencyServices());
                break;

            default:
                win = new LoginView(new UserServices());
                break;
        }
        
        InitView(win, pParent);
    }


    

    public void ClearAllCssProviders(CssProvider provider)
      {
        var screen = Screen.Default;
        StyleContext.RemoveProviderForScreen(screen, provider);
      }

    private void InitView(View win, View? pParent)
    {
      App!.AddWindow(win);
      win.SetDefaultSize(1280, 720);
      if (pParent != null && pParent.IsMaximized) {win.Maximize();}
      win.Show();
      if (pParent != null) {
        // Quitar el estilo aplicado previamente
        this.ClearAllCssProviders(pParent.cssProvider);
        this.App.RemoveWindow(pParent);
        pParent.Destroy();
        }
    }



    public MessageDialog GetMessageDialog(View pParent, string pTitle, string pMessage,Image pImage,
        string pButtonLabel, int pWidth = 400, int pHeight = 300)
    { 
      return new MessageDialog(pParent, pTitle, pMessage, pImage, pButtonLabel);
    }
  }
}
