using System;
using Gtk;
using CryptoTrackApp.src.view;
using CryptoTrackApp.src.services;
using CryptoTrackApp.src.view.Controllers;
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
	    

	    IViewManager vw = ViewManager.GetInstance();
	    vw.App = app;
	    vw.ChangeView(newViewClass: "Login");
            Application.Run();
        }
    }
}
