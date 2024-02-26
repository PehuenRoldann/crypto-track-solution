
using Gtk;
using CryptoTrackApp.src.view;

namespace CryptoTrackApp.src.view_managment {
  public interface IViewManager {
    
    public Application App {get; set;}
    public void ChangeView(string newViewClass, View? oldView=null);
    //public DateSelectionDialog ShowDateSelector();
    public Dialog GetDialog(Window pParent, string pTitle, int pWidth = 400, int pHeight = 300);
  }
}
