using System;
using System.IO;
using System.Security;

namespace CryptoTrackApp.src.utils
{

    public struct SubscriptionKeys
    {
        public const string UserId = "UserId";
        public const string SubscriptionId = "SubscriptionId";
        public const string CurrencyId = "CurrencyId";
        public const string NotificationThreshold = "NotificationUmbral";
        public const string FollowDate = "FollowDate";
        public const string UnfollowDate = "UnfollowDate";
    }

    public struct CryptoCurrencyKeys
    {
        public const string Id = "Id";
        public const string Rank = "Rank";
        public const string Symbol = "Symbol";
        public const string Name = "Name";
        public const string Supply = "Supply";
        public const string MaxSupply = "MaxSupply";
        public const string MarketCapUsd = "MarketCapUsd";  // Asegúrate de usar la nomenclatura correcta para tus claves
        public const string VolumeUsd24Hr = "VolumeUsd24Hr";
        public const string PriceUsd = "PriceUsd";
        public const string ChangePercent24Hr = "ChangePercent24Hr";
        public const string VWap24Hr = "VWap24Hr";
        public const string Explorer = "Explorer";
    }


    public static class Config {
        public static readonly string[] JsonConfArrPath = { "src", "conf.json" }; 
    }

    public readonly struct ConfigurationsKeys {
        public const string DbConnection = "db_connection";
        public const string CoinCapApi = "coin_api_url";
        public const string ApiKey = "coin_cap_api_key";
        public const string MainViewTimeoutRefresh = "timeout_refresh_in_minutes";
    }

    public readonly struct Templates {

        public const string LoginWindow = "LoginWindow";
        public const string MainView = "MainView";
    }

    public readonly struct ViewsIds {
        public const string Login = "login";
        public const string Main = "main";
        public const string Follow = "follow";
        public const string SignUp= "signup";
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
    


    public static class ImagesArrPaths {

        public static readonly string[] CheckMark = { "src", "assets", "images", "check-svgrepo-com.svg"};
        public static readonly string[] CrosskMark = { "src", "assets", "images", "cross-svgrepo-com.svg"};
        public static readonly string[] Warning = { "src", "assets", "images", "warning-svgrepo-com.svg"};
        public static readonly string[] AppLogo = { "src", "assets", "images", "cta_logo_64x64.png" };
    }


    public readonly struct PixBufs {

        public const string AppLogo = "CryptoTrackApp.src.assets.icons.cta_logo_64x64.png";
    }

    public readonly struct DefaultCurrencyData {
        public const string bitcoinId = "bitcoin";
        public const string bitcoinName = "bitcoin";
    }

    
}