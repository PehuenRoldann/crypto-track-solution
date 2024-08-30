using Gtk;

namespace CryptoTrackApp.src.view.Windows
{

  public abstract class View : ApplicationWindow 
  {

    public CssProvider cssProvider = new CssProvider();

    public View(string TEMPLATE) : this(new Builder(TEMPLATE + ".glade"),TEMPLATE) {
      
      this.ConfigEventHandlers();
      this.ConfigImages();
    }

    private View(Builder builder, string Template) : base (builder.GetRawOwnedObject(Template)) {

      builder.Autoconnect(this);
      DeleteEvent += Window_DeleteEvent;     
    }

    public void SetStyle(string cssPath) { // Method for switching the color pallet.

      cssProvider.LoadFromPath(cssPath);
      StyleContext.AddProviderForScreen(Gdk.Screen.Default, cssProvider, 1000);
    }

    

    private void Window_DeleteEvent(object sender, DeleteEventArgs a)
    {
      Application.Quit();
    }

    public abstract void ConfigEventHandlers(); // Use this method to configure the events for your buttons.

    public abstract void ConfigImages(); // use this method to configure your images.
  }
}
