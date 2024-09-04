using System;
using IO = System.IO;
using Gtk;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace CryptoTrackApp.src.view.Windows
{

  public abstract class View : ApplicationWindow 
{
      public string _basePath = AppDomain.CurrentDomain.BaseDirectory;
      public CssProvider cssProvider = new CssProvider();
      private readonly string[] LOGO_PATH = { "src", "assets", "images", "cta_logo_64x64.ico" };

      public View(string TEMPLATE) : this(new Builder(TEMPLATE + ".glade"), TEMPLATE) 
      {
          this.ConfigEventHandlers();
          this.ConfigImages();
      }

      private View(Builder builder, string template) : base(builder.GetRawOwnedObject(template)) 
      {
          builder.Autoconnect(this);
          try
          {
              if (ExistResource(LOGO_PATH))
              {
                  this.SetIconFromFile(GetAbsolutePath(LOGO_PATH));
              }
              else
              {
                  Console.WriteLine("Logo file not found.");
              }
          }
          catch (Exception error)
          {
              Console.WriteLine($"Failed to set window icon: {error.Message}");
          }
          DeleteEvent += Window_DeleteEvent;
      }

      public void SetStyle(string cssPath) 
      {
          cssProvider.LoadFromPath(cssPath);
          StyleContext.AddProviderForScreen(Gdk.Screen.Default, cssProvider, 1000);
      }

      private void Window_DeleteEvent(object sender, DeleteEventArgs a)
      {
          Application.Quit();
      }

      public abstract void ConfigEventHandlers(); 
      public abstract void ConfigImages(); 

      public bool ExistResource(string[] pPath) 
      {
          string absPath = GetAbsolutePath(pPath);
          return IO.File.Exists(absPath);
      }

      public string GetAbsolutePath(string[] pPath) 
      {
          return IO.Path.Combine(pPath.Prepend(_basePath).ToArray());
      }
  }

}
