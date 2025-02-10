using Gtk;
using System;

namespace CryptoTrackApp.src.utils
{
    public static class GtkUtils {

        public static Gtk.Window? GetParentWindow (Gtk.Widget widget) {

            var parent = widget.Parent;
            if (parent == null) {
                return null;
            }
            else if( parent != null && parent is Gtk.Window ) {
                return parent as Window;
            }
            else {
                return GetParentWindow(parent!);
            }
        }
    }
}