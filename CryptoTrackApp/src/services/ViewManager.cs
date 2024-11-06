using System;
using Gtk;
using CryptoTrackApp.src.view.Windows;
using CryptoTrackApp.src.services;
using MessageDialog = CryptoTrackApp.src.view.Windows.MessageDialog;
using Gdk;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;


namespace CryptoTrackApp.src.services
{
  public class ViewManager : IViewManager
  {
    public string UserId {get; set;}

    private Application? _app;

    private static ViewManager _instance;

    private static readonly object _lock = new Object();

    private IDictionary<ViewId, View?> _viewsDty = new Dictionary<ViewId, View?>();

    
    private ViewManager() {

      foreach (ViewId vid in Enum.GetValues(typeof(ViewId))) {

       this._viewsDty.Add(vid, null);
      }

    }


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


    public void ShowView(ViewId pViewId, View? pParent = null)
    {

      if(this._viewsDty[pViewId] != null) {

        this._viewsDty[pViewId]!.Present();
      }
      else {

      View win;
      switch (pViewId)
      {
        case ViewId.Login:
          win = new LoginView(new UserServices());
          break;
        
        case ViewId.SignUp:
          win = new SignUpView(new UserServices());
          break;
        
        case ViewId.Main:
          win = new MainView(this.UserId, new SubscriptionServices(),
           new CurrencyServices(), new PloterService());
          break;
        
        case ViewId.Follow:
          win = new FollowView(new SubscriptionServices(), new CurrencyServices());
          break;
        
        default:
          win = new LoginView(new UserServices());
          break;
      }

      this._viewsDty[pViewId] = win;

      this.InitView(win, pParent);
      }

    }
    

    public void ClearAllCssProviders(CssProvider provider)
      {
        var screen = Screen.Default;
        StyleContext.RemoveProviderForScreen(screen, provider);
      }

    private void InitView(View win, View? pParent)
    {
      this.App!.AddWindow(win);
      win.SetDefaultSize(1280, 720);
      /* win.Move(0,0); */
      if (pParent != null && pParent.IsMaximized) {win.Maximize();}
      win.Show();
      Console.WriteLine("Hola");
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



    /// <summary>
    /// Removes the view with the given id
    /// </summary>
    /// <param name="pViewId">View Id</param>
    public void CloseView (ViewId pViewId) {

      this._viewsDty[pViewId]!.Close();

      bool allNull = true;

      foreach (View? view in this._viewsDty.Values) {

        allNull = view == null && allNull;
      }

      if (allNull) {

        Application.Quit();
      }

    }
  }
}
