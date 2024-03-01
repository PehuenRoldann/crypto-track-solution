using System;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;
using CryptoTrackApp.src.view.Components;
using Pango;

namespace CryptoTrackApp.src.view.Windows
{
    public class MainView : View
    {
        [UI] private Image _logoImg;
        [UI] private Box _panel;

        private string LOGO_PATH = "./src/assets/images/logo_v2.png";
        public MainView() : base("MainView")
        {
            this.ConfigNotificationsArea();
            this.ConfigSubsList();
            DeleteEvent += OnDelete;
            this.CSS_PATH_DARK = "./src/css/main_view.css";
            this.SetStyle("dark");
            this._panel.ShowAll();
        }

// ----------- INITIAL CONFIGURATIONS ---------------------------------------
        private void OnDelete(object o, DeleteEventArgs args)
        {
            Application.Quit();
        }

        public override void ConfigEventHandlers()
        {
            Console.WriteLine("Configuring event handlers.......");
        }

        public override void ConfigImages()
        {
            this._logoImg.File = this.LOGO_PATH;
            Console.WriteLine("Configuring images...");

        }

        public void ConfigSubsList()
        {
            CryptoTreeViewComponent subsTree = new CryptoTreeViewComponent();
            this._panel.Add(subsTree);
            this._panel.ReorderChild(subsTree, 1);
            subsTree.Expand = true;
            subsTree.Halign = Align.Center;
            subsTree.Valign = Align.Center;
            subsTree.StyleContext.AddClass("subs-tree");
            //this._panel.ShowAll();

        }

        public void ConfigNotificationsArea()
        {
            NotificationsAreaComponent notifArea = new NotificationsAreaComponent();
            notifArea.StyleContext.AddClass("notification-area");
            this._panel.Add(notifArea);
            this._panel.SetSizeRequest(300, -1);
        }
    }
}