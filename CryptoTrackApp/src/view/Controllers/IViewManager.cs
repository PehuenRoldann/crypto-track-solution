
using Gtk;
using Gdk;
using CryptoTrackApp.src.view.Windows;

using MessageDialog = CryptoTrackApp.src.view.Windows.MessageDialog;

namespace CryptoTrackApp.src.view.Controllers
{

  public interface IViewManager {
    
    public Application App {get; set;}
    public void ChangeView(string newViewClass, View? oldView=null);
    //public DateSelectionDialog ShowDateSelector();
    public MessageDialog GetMessageDialog(View pParent, string pTitle, string pMessage,
      Image pImage, string pButtonLabel, int pWidth = 400, int pHeight = 300);
  }
}