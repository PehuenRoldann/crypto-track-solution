using Gdk;

namespace CryptoTrackApp.src.view {
  public static class IconManager {
    
    public static Gdk.Pixbuf valid_icon = Gdk.Pixbuf.LoadFromResource("CryptoTrackApp.src.assets.icons.check_valid.png");
    public static Gdk.Pixbuf invalid_icon = Gdk.Pixbuf.LoadFromResource("CryptoTrackApp.src.assets.icons.check_invalid.png");
  }
}
