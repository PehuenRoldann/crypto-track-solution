
using Gtk;
using Gdk;
using CryptoTrackApp.src.view;

namespace CryptoTrackApp.src.view_managment {
  public interface IViewManager {
    
    public Application App {get; set;}
    public void ChangeView(string newViewClass, View? oldView=null);
    //public DateSelectionDialog ShowDateSelector();
    public CryptoTrackApp.src.view.MessageDialog GetMessageDialog(Gtk.Window pParent, string pTitle, string pMessage,
      Image pImage, string pButtonLabel, int pWidth = 400, int pHeight = 300);
  }
}
