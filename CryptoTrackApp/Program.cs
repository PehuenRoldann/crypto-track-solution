using System;
using Gtk;
using CryptoTrackApp.src.view;

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

            var win = new LoginWindow();
            app.AddWindow(win);

            win.Show();
            Application.Run();
        }
    }
}
