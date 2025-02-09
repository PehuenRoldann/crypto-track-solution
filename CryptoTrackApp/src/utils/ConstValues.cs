using System.IO;
using System.Security;

namespace CryptoTrackApp.src.utils
{

    public readonly struct Templates {

        public const string LoginWindow = "LoginWindow";
        public const string MainView = "MainView";
    }

    public static class CssFilesPaths {
        public static readonly string[] LoginWindowCss = { "src", "css", "login_window.css" };
        public static readonly string[] MainViewCss = { "src", "css", "main_view.css" };
        public static readonly string[] FollowViewCss = { "src", "css", "follow_view.css" };
        public static readonly string[] SignUpWindowCss = { "src", "css", "SignUpWindow.css" };
    }

    public readonly struct MainViewClases {
        public const string SubsTree = "subs-tree";
        public const string ConfrimationDialogCssClass = "confirmation-dialog";
    }

    public readonly struct ConfrimationDialogCssClases {
        
        public const string ConfrimationDialog = "confirmation-dialog";
        public const string ConfirmationBtn = "confirmation-btn";
        public const string DialogBtn = "confirmation-dialog-btn";
        public const string CancelBtn = "cancel-btn";
    }

    public readonly struct IconsPaths {
        public const string UnfllowIconPath = "CryptoTrackApp.src.assets.icons.red_circle_unfollow.png";
    }

    public static class AssetsArrPaths {
        public static readonly string[] CurrenciesImages = { "src", "assets", "images", "currency" };
        public static readonly string[] CurrenciesIcons = { "src", "assets", "icons", "currency"};
    }
    
}