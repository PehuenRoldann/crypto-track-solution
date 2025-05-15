using CryptoTrackApp.src.utils;

namespace CryptoTrackApp.src.view.helpers 
{
    public class DesktopCryptoNotifier : ICryptoNotifier
    {
        public void Notify(string message)
        {
            var escapedMsg = message.Replace("\"", "\\\"");
            System.Diagnostics.Process.Start("notify-send", $"\"CryptoTrack\" \"{escapedMsg}\"");
        }
    }

}