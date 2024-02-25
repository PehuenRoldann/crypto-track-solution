using Gtk;

namespace CryptoTrackApp.src.view {

  public abstract class View : Gtk.Window 
  {
    public string CSS_PATH_DARK = "";
    public string CSS_PATH_LIGHT = "";
    private string _currentStyle = "";
    private CssProvider cssProvider = new CssProvider();

    public View(string TEMPLATE) : this(new Builder(TEMPLATE + ".glade"),TEMPLATE) {
      
      this.ConfigButtons();
      this.ConfigImages();
    }

    private View(Builder builder, string Template) : base (builder.GetRawOwnedObject(Template)) {

      builder.Autoconnect(this);
      DeleteEvent += Window_DeleteEvent;     
    }

    public string CurrentStyle {get {return this._currentStyle;}}

    public void SetStyle(string pStyle) { // Method for switching the color pallet.
      
      string cssPath = "";

      switch (pStyle.ToLower()) {
	case "dark":
	  cssPath = this.CSS_PATH_DARK;
	  this._currentStyle = "dark";
	  break;
	case "light":
	  cssPath = this.CSS_PATH_LIGHT;
	  this._currentStyle = "light";
	  break;
	default:
	  cssPath = this.CSS_PATH_DARK;
	  this._currentStyle = "default";
	  break;
      }

      cssProvider.LoadFromPath(cssPath);
      StyleContext.AddProviderForScreen(Gdk.Screen.Default, cssProvider, 1000);
    }

    private void Window_DeleteEvent(object sender, DeleteEventArgs a)
    {
      Application.Quit();
    }

    public abstract void ConfigButtons(); // Use this method to configure the events for your buttons.

    public abstract void ConfigImages(); // use this method to configure your images.
  }
}
