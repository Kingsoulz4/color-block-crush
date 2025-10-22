using System.Collections;
using System.Collections.Generic;
using Firebase.Analytics;
using AdjustSdk;
using Analytics;
using Yoolax.Framework;
using static MaxSdkCallbacks;
using System.Drawing;


#if APPLOVIN_ADMOB
using GoogleMobileAds.Api;
#endif
using UnityEngine;

public static class TrackingRevenueConnector
{
    public const float IapReduceRate = 0.7f;
    public enum RevenueSource
    {
        Admob,
        Any
    }
#if APPLOVIN_ADMOB
    #region Admob
    public static void SendRevenue_ToFirebase_Admob(AdValue adValue, string ad_source)
    {
        if (adValue != null)
        {
            double revenue = ConvertRevenue(adValue.Value, RevenueSource.Admob);
            #if UNITY_FIREBASE
            var impressionParameters = new[] {
                new Parameter("ad_platform", Application.platform == RuntimePlatform.Android ? "Android" : "IOS"),
                new Parameter("ad_source", "admob"),
                new Parameter("ad_format","Open App"),
                new Parameter("value", revenue),
                new Parameter("currency", "USD"),
            };

            AdsAnalyticStruct adAnalyticStruct = new AdsAnalyticStruct();
            adAnalyticStruct = adAnalyticStruct.SetBaseAd("OPEN APP", "Admod", ad_source)
                .SetAdImpression(1, (double)(revenue / 1000000f));
            Server.Get<OnAdImpressionEventLog>().Dispatch(adAnalyticStruct);
            //FirebaseManager.Instance.LogRevenueToAdmob(impressionParameters);            
            //Debug.LogError("Send To Firebase: " + revenue);
            #endif
        }
    }
    public static void SendRevenue_ToAppflyer_Admob(AdValue adValue)
    {
        if (adValue != null)
        {
            double revenue = ConvertRevenue(adValue.Value, RevenueSource.Admob);

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add(FirebaseAnalytics.ParameterAdSource, "admob");
            dic.Add(FirebaseAnalytics.ParameterAdFormat, "unit");

            AppsFlyer.logAdRevenue(new AFAdRevenueData("admob", MediationNetwork.GoogleAdMob, "USD", revenue), dic);
            Debug.LogError("Send Admob Revenue To Appsflyer: " + revenue);
        }
    }

    public static void SendRevenueAdmob_To_Facebook(AdValue adValue)
    {
        if (adValue != null)
        {
            float revenue = (float)ConvertRevenue(adValue.Value, RevenueSource.Admob);
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add(AppEventParameterName.Currency, "USD");
            FB.LogAppEvent("AdImpression", revenue, dic);
        }
    }

    #endregion
#endif
    
    #region Applovin
    public static void SendRevenue_ToFirebase_MaxApplovin(MaxSdkBase.AdInfo adValue)
    {
        //FirebaseManager.Instance.LogRevenueToMax(adValue);
        AdsAnalyticStruct adAnalyticStruct = new AdsAnalyticStruct();
        adAnalyticStruct = adAnalyticStruct.SetBaseAd(adValue.AdFormat, "Max", adValue.NetworkName)
            .SetAdImpression(1, ConvertRevenue(adValue.Revenue, RevenueSource.Any));
        Server.Get<OnAdImpressionEventLog>().Dispatch(adAnalyticStruct);
    }

    //public static void SendRevenue_ToAppflyer_Admob(GoogleMobileAds.Api.AdValue adValue, string adSource, string adFormat)
    //{
    //    double revenue = adValue.Value / 1000000f;

    //    Dictionary<string, string> dic = new Dictionary<string, string>();
    //    dic.Add(FirebaseAnalytics.ParameterAdSource, adSource);
    //    dic.Add(FirebaseAnalytics.ParameterAdFormat, adFormat);

    //    AppsFlyer.logAdRevenue(new AFAdRevenueData("admob", MediationNetwork.GoogleAdMob, "USD", revenue), dic);

    //}

    //public static void SendRevenue_ToFirebase_Admob(GoogleMobileAds.Api.AdValue adValue, string adSource, string adFormat)
    //{
    //    double? revenue = adValue.Value;        
    //    var impressionParameters = new[]
    //    {
    //            new Parameter("ad_platform", "ADMOD"),
    //            new Parameter("ad_source",adSource),                
    //            new Parameter("ad_format", adFormat),
    //            new Parameter("value", (float)revenue/ 1000000f),
    //            new Parameter("currency", "USD"),
    //    };
    //    Debug.Log("Admob Ad Source " + adSource);
    //    FirebaseManager.Instance.LogRevenueToAdmob(impressionParameters);       
    //}

    // public static void SendRevenue_ToAppflyer_MaxApplovin(MaxSdkBase.AdInfo adValue)
    // {
    //     if (adValue != null)
    //     {
    //         double revenue = ConvertRevenue(adValue.Revenue, RevenueSource.Any);
    //
    //         Dictionary<string, string> dic = new Dictionary<string, string>();
    //         dic.Add(FirebaseAnalytics.ParameterAdSource, "applovin");
    //         dic.Add(FirebaseAnalytics.ParameterAdFormat, "unit");
    //
    //         AppsFlyer.logAdRevenue(new AFAdRevenueData("applovin", MediationNetwork.ApplovinMax, "USD", revenue), dic);
    //
    //         //Debug.LogError("Send To Appsflyer: " + revenue);
    //     }
    //
    // }
    // public static void SendRevenue_ToFacebook_MaxApplovin(MaxSdkBase.AdInfo adValue)
    // {
    //     if (adValue != null)
    //     {
    //         float revenue = (float)ConvertRevenue(adValue.Revenue, RevenueSource.Any);
    //
    //         Dictionary<string, object> dic = new Dictionary<string, object>();
    //         dic.Add(AppEventParameterName.Currency, "USD");
    //         FB.LogAppEvent("AdImpression", revenue, dic);
    //     }
    //
    // }

    //public static void SendRevenueAdmob_To_Facebook(AdValue adValue)
    //{
    //    if (adValue != null)
    //    {
    //        float revenue = (float)ConvertRevenue(adValue.Value, RevenueSource.Admob);
    //        Dictionary<string, object> dic = new Dictionary<string, object>();
    //        dic.Add(AppEventParameterName.Currency, "USD");
    //        FB.LogAppEvent("AdImpression", revenue, dic);
    //    }
    //}

    #endregion

    // public static void SendRevenue_IAP_ToFaceBook(ProductType productType)
    // {
    //     QonversionUnity.Product product = QOnController.Instance.qOnData.Price(productType);
    //     if (product != null)
    //     {
    //         if (product.Price <= 0)
    //             return;
    //
    //         if (string.IsNullOrEmpty(product.CurrencyCode))
    //             return;
    //
    //         PurchaseModel purchaseModel = product.ToPurchaseModel();
    //         if (purchaseModel != null)
    //         {
    //             //Debug.LogError(purchaseModel.ProductId + "//" + (decimal)product.Price + "//" + product.CurrencyCode);
    //
    //             var iapParameters = new Dictionary<string, object>();
    //             iapParameters.Add("product_id", purchaseModel.ProductId);
    //
    //             double reducePrice = product.Price;
    //             FB.LogPurchase(
    //               (float)reducePrice,
    //               product.CurrencyCode,
    //               iapParameters
    //             );
    //         }
    //     }
    // }

    public static double ConvertRevenue(double revenue, RevenueSource revenueSource)
    {
        if (revenueSource == RevenueSource.Admob)
        {
            double value = revenue / 1000000;
            if (value >= 1000)
            {
                Debug.LogError("Impression Fail: " + value + "////////////////// Check /////////////////////////------------------------------------------");
            }

            // Debug.LogError("Impression Success: " + value);
            return value;
        }
        else
        {
            double value = revenue;
            if (value >= 1000)
            {
                Debug.LogError("Impression Fail: " + value + "////////////////// Check /////////////////////////------------------------------------------");
            }
            // Debug.LogError("Impression Success: " + value);
            return value;
        }
    }

}
