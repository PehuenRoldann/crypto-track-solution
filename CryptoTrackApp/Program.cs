using System;
using Gtk;
using ViewManager = CryptoTrackApp.src.services.ViewManager;
namespace CryptoTrackApp
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.Init();
            var app = new Application("org.CryptoTrackApp.CryptoTrackApp", GLib.ApplicationFlags.None);
            app.Register(GLib.Cancellable.Current);
	    

            ViewManager vw = ViewManager.GetInstance();
            vw.App = app;
            // vw.ShowFollowView();
            vw.ShowView("login");
            Application.Run();
        }
    }
}
