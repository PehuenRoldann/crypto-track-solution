using System;
using Gtk;
using ViewManager = CryptoTrackApp.src.services.ViewManager;
using CryptoTrackApp.src.utils;

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
            new Logger().Log("INIT - Starting Simple Track Application");
            Application.Run();
        }
    }
}
