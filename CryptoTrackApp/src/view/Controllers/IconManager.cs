using Gdk;

namespace CryptoTrackApp.src.view.Controllers
{
  public static class IconManager
  {
    
    public static Gdk.Pixbuf valid_icon = Gdk.Pixbuf.LoadFromResource("CryptoTrackApp.src.assets.icons.check_valid.png");
    public static Gdk.Pixbuf invalid_icon = Gdk.Pixbuf.LoadFromResource("CryptoTrackApp.src.assets.icons.check_invalid.png");
    public static Gdk.Pixbuf wait_96 = Gdk.Pixbuf.LoadFromResource("CryptoTrackApp.src.assets.icons.wait-96.png");
    public static Gdk.Pixbuf wait_48 = Gdk.Pixbuf.LoadFromResource("CryptoTrackApp.src.assets.icons.wait-48.png");
    public static PixbufAnimation checkmark_light_animation = new PixbufAnimation("src/assets/gifs/checkmark_light.gif");
    public static Gdk.Pixbuf not_found = Gdk.Pixbuf.LoadFromResource("CryptoTrackApp.src.assets.icons.img_not_found.png");
  }
}
