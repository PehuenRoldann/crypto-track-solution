
using Gtk;
using Gdk;
using CryptoTrackApp.src.view.windows;

using MessageDialog = CryptoTrackApp.src.view.components.MessageDialog;

namespace CryptoTrackApp.src.view.helpers
{

  public interface IViewManager {
    
    public Application App {get; set;}
    public void ShowView(string pViewType, View? pParent = null);
    public MessageDialog GetMessageDialog(View pParent, string pTitle, string pMessage,
    Image pImage, string pButtonLabel, int pWidth = 400, int pHeight = 300);
  }
}
