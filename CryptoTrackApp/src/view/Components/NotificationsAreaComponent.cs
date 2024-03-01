using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gtk;

namespace CryptoTrackApp.src.view.Components
{
    public class NotificationsAreaComponent : Box
    {

        private Label notificationLabel;

        public NotificationsAreaComponent() {
            this.ConfigNotificationLabel();
        }

        public void ConfigNotificationLabel () {
            
            this.notificationLabel = new Label();
            this.notificationLabel.Text = "Notifications";
            this.notificationLabel.UseUnderline = true;
            var fontDescription = Pango.FontDescription.FromString("Arimo 12");
            this.notificationLabel.ModifyFont(fontDescription);
            this.notificationLabel.WidthRequest = 300;
            this.Add(this.notificationLabel);
    
        }
        
    }
}