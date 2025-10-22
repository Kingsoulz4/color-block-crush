using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Analytics
{
    public static class AnalyticUtils
    {
        #region User Property

        static List<string> currentName = new List<string>();

        public static UserPropertiesStruct GetCurrentUserProperty()
        {
            UserDataManager.UserData userData = UserDataManager.LoadUserData();

            UserPropertiesStruct userPropertyStruct = new UserPropertiesStruct();

            userPropertyStruct.currentLevel = userData.level;
            userPropertyStruct.mode = Mode.normal;
            userPropertyStruct.connectType = GetConnectionTypeFromUnityApi();
            userPropertyStruct.firebaseExp = "Null";
            userPropertyStruct.isIapUser = userData.buyIapCount > 0 ? 1 : 0;
            userPropertyStruct.iapCount = userData.buyIapCount;
            userPropertyStruct.winStreak = userData.winStreak;
            userPropertyStruct.loseStreak = userData.loseStreak;
            userPropertyStruct.balanceCoin = userData.gold;
            userPropertyStruct.balanceHeart = userData.heart;
            userPropertyStruct.balanceAddTray = userData.addTrayBooster;
            userPropertyStruct.balanceHand = userData.handBooster;
            userPropertyStruct.balanceShuffle = userData.shuffleBooster;
            userPropertyStruct.balanceSuperShoot = userData.magnetBooster;
            userPropertyStruct.balanceRevive = 0;

            return userPropertyStruct;
        }

        public static Analytics.ConnectType GetConnectionTypeFromUnityApi()
        {
            var reach = Application.internetReachability;
            switch (reach)
            {
                case NetworkReachability.NotReachable:
                    return ConnectType.offline;
                case NetworkReachability.ReachableViaLocalAreaNetwork:
                    return ConnectType.wifi;
                case NetworkReachability.ReachableViaCarrierDataNetwork:
                    return ConnectType.mobile_data;
                default:
                    return ConnectType.unknown;
            }
        }

        #endregion

        #region Level

        private const int maxLevel = 250;

        public static LevelAnalyticStruct SetBaseLevel(this LevelAnalyticStruct levelBaseStruct)
        {
            levelBaseStruct.SetBaseLevel(UserDataManager.Level,
                0, Mode.normal,
                UserDataManager.PlayIndex, UserDataManager.LoseIndex);
            return levelBaseStruct;
        }        

        #endregion

        #region Resource        

        public static string ConnectString(string[] inputString)
        {
            if (inputString == null || inputString.Length == 0) return string.Empty;
            
            int nonEmptyCount = 0;
            int contentLen = 0;
            for (int i = 0; i < inputString.Length; i++)
            {
                var s = inputString[i];
                if (!string.IsNullOrEmpty(s))
                {
                    nonEmptyCount++;
                    contentLen += s.Length;
                }
            }

            if (nonEmptyCount == 0) return string.Empty;
            if (nonEmptyCount == 1)
            {
                for (int i = 0; i < inputString.Length; i++)
                    if (!string.IsNullOrEmpty(inputString[i]))
                        return inputString[i];
            }

            const int sepLen = 1;
            int totalLen = contentLen + sepLen * (nonEmptyCount - 1);

            return string.Create(totalLen, inputString, (span, arr) =>
            {
                int pos = 0;
                bool first = true;
                for (int i = 0; i < arr.Length; i++)
                {
                    var s = arr[i];
                    if (string.IsNullOrEmpty(s)) continue;

                    if (!first)
                    {
                        span[pos++] = ',';
                    }
                    else first = false;

                    s.AsSpan().CopyTo(span.Slice(pos));
                    pos += s.Length;
                }
            });
        }

        public static ResourceType GetCurrencyTypeFromCurrencyName(string resourceName)
        {
            switch (resourceName)
            {
                case "gold":
                case "coin":               
                    return ResourceType.currency;
                case "undo":
                case "shuffle":
                case "magnet":
                case "add_space":
                    return ResourceType.booster;
                case "accessory":
                case "infinity_heart":
                case "heart":
                    return ResourceType.item;
                default:
                    return ResourceType.item;
            }
        }

        #endregion

        #region IAP

        public const string prefixPlacementHome = "home_";
        public const string prefixPlacementGameplay = "ingame_";

        public static InAppPurchaseAnalyticStruct SetBaseIAP(this InAppPurchaseAnalyticStruct iapBaseStruct, IAPShowType showType,
            TriggerType triggerType, string packName)
        {
            iapBaseStruct.SetBaseIAP(UiHolderManager.Instance.GetCurrentPlacement(), showType, triggerType, packName);
            return iapBaseStruct;
        }

        #endregion

        #region Ads

        public static AdsAnalyticStruct SetBaseAd(this AdsAnalyticStruct adBaseStruct, string adFormat, string adPlatform, string adNetwork)
        {
            string adNetworkConvert = ShortenAdNetwoekAdapter(adNetwork);
            adBaseStruct.SetBaseAd(adFormat, adPlatform, adNetworkConvert, AnalyticManager.Instance.adPlacement /*UiHolderManager.Instance.GetCurrentPlacement()*/);
            return adBaseStruct;
        }

        private static readonly (string key, string label)[] KeywordMap =
    {
        (".admob.",      "Google AdMob"),
        (".applovin.",   "AppLovin"),
        (".pangle.",     "Pangle"),
        (".facebook.",   "Facebook"),   // aka Meta Audience Network
        (".meta.",       "Meta"),
        (".unity.",      "Unity Ads"),
        (".adcolony.",   "AdColony"),
        (".chartboost.", "Chartboost"),
        (".vungle.",     "Vungle"),
        (".ironsource.", "ironSource"),
        (".mintegral.",  "Mintegral"),
        (".inmobi.",     "InMobi"),
        (".tapjoy.",     "Tapjoy"),
        (".mopub.",      "MoPub"),
        (".moloco.",     "Moloco"),
        (".mytarget.",   "myTarget"),
        (".line.",       "LINE"),
        (".imobile.",    "i-mobile"),
    };

        public static string ShortenAdNetwoekAdapter(string adapterClassName)
        {
            if (string.IsNullOrWhiteSpace(adapterClassName))
                return string.Empty;

            var lower = adapterClassName.ToLowerInvariant();

            foreach (var (key, label) in KeywordMap)
            {
                if (lower.Contains(key))
                    return label;
            }

            var lastDot = adapterClassName.LastIndexOf('.');
            var last = lastDot >= 0 ? adapterClassName.Substring(lastDot + 1) : adapterClassName;

            var simplified = Regex.Replace(last, "(?i)(Mediation)?Adapter$", "");

            if (string.Equals(simplified, "Admob", StringComparison.OrdinalIgnoreCase)) return "AdMob";
            if (string.Equals(simplified, "Unity", StringComparison.OrdinalIgnoreCase)) return "Unity Ads";

            return simplified;
        }

        #endregion        
    }
}

