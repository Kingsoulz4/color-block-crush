using Firebase;
using Firebase.Analytics;
using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.Networking;
using Yoolax.Framework;

//using static TrackingRevenueConnector;

public class FirebaseManager : SingletonDontDestroyMono<FirebaseManager>
{
    private static bool isInited;
    [SerializeField] private bool isDebug;
    public static event Action OnFetchRemoteComplete;

    Queue<EventData> eventQueues = new Queue<EventData>();

    Parameter[] impressionParameters = new[]
    {
        new Firebase.Analytics.Parameter("ad_platform",
            Application.platform == RuntimePlatform.Android ? "Android" : "IOS"),
        new Firebase.Analytics.Parameter("value", 0),
        new Firebase.Analytics.Parameter("currency", "USD"),
        new Firebase.Analytics.Parameter("ad_source", ""),
        new Firebase.Analytics.Parameter("ad_format", "")
    };

    public void Init()
    {
        isInited = false;
        OnInit();
#if UNITY_IOS
impressionParameters[0] = new Firebase.Analytics.Parameter("ad_platform", "Ios");
#endif
    }

    private void Update()
    {
        // if (isInited)
        // {
            if (eventQueues.Count > 0)
            {
                EventData eventData = eventQueues.Dequeue();
                eventData.FireEvent();
            }
        //}
    }

    void OnInit()
    {
#if UNITY_FIREBASE
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                Debug.Log("CheckAndFixDependenciesAsync success!");

                FetchRemoteConfig();

                isInited = true;
                databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
                Debug.Log("databaseReference success!");
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                    "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });
#endif
    }

    void FetchRemoteConfig()
    {
#if UNITY_FIREBASE
        var fetchTask = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(new System.TimeSpan(0));

        fetchTask.ContinueWith(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.Log("Log::: Fetch Remote Config: Failed........");
            }
            else
            {
                Debug.Log("Log::: Fetch Remote Config: Completed........");
            }

            Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync();
            RefrectProperties();
        });
#endif
    }

    private void RefrectProperties()
    {
#if UNITY_FIREBASE
#endif
    }

    public void AddQueueProperty(string propertyName, string value)
    {
        EventData queue = new PropertyEventData(propertyName, value);
        eventQueues.Enqueue(queue);
    }

    private void AddEvent(string eventName)
    {
        EventData queue = new NormalEventData(eventName);
        eventQueues.Enqueue(queue);
    }

    public void AddEvent(string eventName, Dictionary<string, object> paramData, params Parameter[] parameters)
    {
        EventData queue = new ParatermterEventData(eventName, paramData, parameters);
        eventQueues.Enqueue(queue);
    }

    public void LogEvent(string name)
    {
        AddEvent(name);
        if (isDebug)
        {
            Debug.LogError(name);
        }
    }

    public void LogEventPara(string name, Dictionary<string, object> paramData, params Parameter[] parameters)
    {
        AddEvent(name, paramData, parameters);
        if (isDebug)
        {
            Debug.LogError(name);
        }
    }

//     public void LogRevenueToMax(MaxSdkBase.AdInfo adValue)
//     {
//         if (adValue != null)
//         {
//             double revenue =
//                 TrackingRevenueConnector.ConvertRevenue(adValue.Revenue, TrackingRevenueConnector.RevenueSource.Any);
//             impressionParameters[1] = new Firebase.Analytics.Parameter("value", revenue);
//             impressionParameters[3] = new Firebase.Analytics.Parameter("ad_source", adValue.NetworkName);
//             impressionParameters[4] = new Firebase.Analytics.Parameter("ad_format", adValue.AdFormat);
//             //AddEvent("ad_impression", impressionParameters);
//             Debug.Log("Ad Impression Revenue " + revenue);
//
// #if UNITY_IOS
//             revenue = revenue * DataManager.Instance.RemotePrefs.revenue_coefficient;
//             impressionParameters[1] = new Firebase.Analytics.Parameter("value", revenue);
//             AddEvent("ad_impression_1", impressionParameters);
//             Debug.Log("Ad Impression 1 Revenue " + revenue);
// #endif
//         }
//         //Debug.Log("Max Ad Value " + adValue);
//     }

    public void LogRevenueToAdmob(Parameter[] param)
    {
        if (param.Length > 0)
        {
            //AddEvent("ad_impression", param);
        }

        Debug.Log("Admob Ad Value " + param[2]);
    }
}

public enum RateAction
{
    not_now,
    submit
}

public enum StatusType
{
    success,
    fail
}

public enum IAPPoint
{
    shop,
    no_ads_pack,
    starter_pack,
    extra,
    pack
}

public enum CoinInPoint
{
    none,
    win,
    win_x2_reward,
    restore_pack,
    starter_pack,
    shop_coin,
    shop_starter_pack,
    shop_free_coin,
    no_ads_pack,
    collect,
    iap_extra
}

public enum CoinOutPoint
{
    none,
    undo,
    add_space,
    shuffle,
    magnet,
    revival,
    heart,
    hat
}

public enum HeartInPoint
{
    time,
    gold,
    ads
}

public enum HeartOutPoint
{
    exit_game,
    kill_app,
    lose,
    replay_game,
}

public enum AdsAction
{
    click_ad,
    ad_show,
    claim_success,
    ad_impression
}

public enum InterPoint
{
    fail,
    next_stage,
    win,
    banner_point
}

public enum RewardPoint
{
    shop_free_gold,
    win_reward,
    unlock_hat,
    undo,
    shuffle,
    add_space,
    magnet,
    revival,
    heart
}

public enum CurrenctType
{
    heart,
    gold
}