using System.IO;

namespace CryptoTrackApp.src.utils
{

    public readonly struct Templates {

        public const string LoginWindow = "LoginWindow";
        public const string MainView = "MainView";
    }

    public static class CssFilesPaths {
        public static string[] LoginWindowCss = { "src", "css", "login_window.css" };
        public static string[] MainViewCss = { "src", "css", "main_view.css" };
        public static string[] FollowViewCss = { "src", "css", "follow_view.css" };
        public static string[] SignUpWindowCss = { "src", "css", "SignUpWindow.css" };
    }

    public readonly struct MainViewClases {
        public const string SubsTree = "subs-tree";
    }
    
}