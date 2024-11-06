
using Gtk;
using Gdk;
using CryptoTrackApp.src.view.Windows;

using MessageDialog = CryptoTrackApp.src.view.Windows.MessageDialog;

namespace CryptoTrackApp.src.services
{

  public enum ViewId 
  {
    Login,
    SignUp,
    Main,
    Follow
  }

  public interface IViewManager {
    
    public Application App {get; set;}
    public void ShowView(ViewId pViewId, View? pParent = null);
    public void CloseView (ViewId pViewId);
    public MessageDialog GetMessageDialog(View pParent, string pTitle, string pMessage,
    Image pImage, string pButtonLabel, int pWidth = 400, int pHeight = 300);
  }
}
