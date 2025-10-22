using System;
using System.Collections;
using UnityEngine;
using AdjustSdk;
using System.Collections.Generic;
using Analytics;
using System.Collections.Concurrent;
using System.Diagnostics;
using ColorBlockCrush;

public class MaxAdsManager : SingletonDontDestroyMono<MaxAdsManager>
{
    private string sdkKey = "nyoXoy93aTt9CkHTWGs5QxP4JzEIS81D5pltmvxKRpUeWnB2128R-XWHUlayuc9zQvrAonVEZ_t5m37o6GE4ol";

    [SerializeField] private Color bannerColor = new Color(0, 0, 0, 0);
    private readonly Dictionary<string, Stopwatch> timers = new Dictionary<string, Stopwatch>();
    ConcurrentDictionary<string, Stopwatch> adPlayTimers = new ConcurrentDictionary<string, Stopwatch>();
    private DateTime startShowAds = DateTime.Now;

    //Interstitial
    int retryAttemptInterstitial;
    //Video
    int retryAttemptRewarded;
    //
    private Action onComplete;
    public bool adsIsShowing;
    public float currentTime;
    public float currentTimeCheck;
    private bool acceptCallback;

    public static float lastAdCloseTime;

    public static float lastTimeAdsShow;


    protected override void Awake()
    {
        base.Awake();
        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
        {
            //async.allowSceneActivation = true;
        };        
        MaxSdk.InitializeSdk();
        UnityEngine.Debug.Log("Init Max Ads");
        adsIsShowing = false;
        InitializeBannerAds(MaxKeys.bannerID);
        InitializeInterstitialAds();
        InitializeRewardedAds();

        LoadInterstitial(MaxKeys.interstitialID);
        LoadRewardedAd(MaxKeys.rewardedID);

        currentTime = Time.unscaledTime;
       
    }

    private void OnDestroy()
    {
        RemoveListenerBannerAds();
        RemoveListenerInterstitialAds();
        RemoveListenerRewardedAds();
    }
 

    public void ShowBanner(string adUnitId)
    {
        MaxSdk.ShowBanner(adUnitId);
    }
    public void HideBanner(string adUnitId)
    {
        MaxSdk.HideBanner(adUnitId);
    }
    public void ShowInterstitialAd(string adUnitId, Action onComplete = null)
    {
        if (ShopManager.Instance.HasPurchasedNoAdsPack)
        {
            return;
        }
        if(UserDataManager.Level < 20) return;
        if (!MaxAdsManager.Instance.IsShowPopupDefault(MaxKeys.interstitialID))
        {
            return;
        }
        UnityEngine.Debug.Log("Show Inter Max");
        if (TestManager.IsCheating && TestManager.IsAdsOff)
        {
            onComplete?.Invoke();
            return;
        }

        this.onComplete = onComplete;
        this.acceptCallback = true;
#if UNITY_EDITOR || UNITY_TEST_ADS
        var popupAds = UIManager.Instance.ShowPopup<PopupAds>(() =>
        {
            currentTime = Time.unscaledTime;
            onComplete?.Invoke();
        });


#else
        if (MaxSdk.IsInterstitialReady(adUnitId))
        {
            PauseCheck();
            MaxSdk.ShowInterstitial(adUnitId);
            adsIsShowing = true;
			lastTimeAdsShow = Time.realtimeSinceStartup;
        }
		else
		{

		}
#endif
    }
    public void ShowRewardedAd(string adUnitId, Action onComplete = null)
    {
        this.onComplete = onComplete;
        this.acceptCallback = false;

        if (TestManager.IsCheating && TestManager.IsAdsOff)
        {
            var popupAds = UIManager.Instance.ShowPopup<PopupAds>(() =>
            {
                currentTime = Time.unscaledTime;
                onComplete?.Invoke();
            });
            return;
        }

#if UNITY_EDITOR || UNITY_TEST_ADS
        UIManager.Instance.ShowPopup<PopupAds>(() =>
        {
            currentTime = Time.unscaledTime;
            onComplete?.Invoke();
        });
#else

        if (MaxSdk.IsRewardedAdReady(adUnitId))
        {
			PauseCheck();
            MaxSdk.ShowRewardedAd(adUnitId);
            adsIsShowing = true;
			FirebaseManager.Instance.LogEvent("reward_max");
			lastTimeAdsShow = Time.realtimeSinceStartup;
		}
        else
		{
            if (InterstitialAvailable(MaxKeys.interstitialID))
            {
                PauseCheck();
                this.acceptCallback = true;
                MaxSdk.ShowInterstitial(MaxKeys.interstitialID);
                adsIsShowing = true;
                //Debug.LogError("Show Inter");

				FirebaseManager.Instance.LogEvent("reward_inter");
				lastTimeAdsShow = Time.realtimeSinceStartup;
            }
            else
            {
				//Debug.LogError("Inter reward Fail");
                UIManager.Instance.ShowPopup<PopupMiniNoti>(null).Show("Ad not available");
            }
        }
#endif
    }
    public bool InterstitialAvailable(string adUnitId)
    {

        if (Application.isEditor)
        {
            return true;
        }
        else
        {
            return MaxSdk.IsInterstitialReady(adUnitId);
        }

    }
    public bool RewardedAvailable(string adUnitId)
    {
#if UNITY_EDITOR
        return true;
#else
        return MaxSdk.IsRewardedAdReady(adUnitId);
#endif
    }



    #region Banner

    public void InitializeBannerAds(string adUnitId)
    {
        // Banners are automatically sized to 320�50 on phones and 728�90 on tablets
        // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
        MaxSdk.CreateBanner(adUnitId, MaxSdkBase.BannerPosition.BottomCenter);
        MaxSdk.SetBannerExtraParameter(adUnitId, "adaptive_banner", "false");

        // Set background or background color for banners to be fully functional
        MaxSdk.SetBannerBackgroundColor(adUnitId, bannerColor);

        MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdLoadFailedEvent;
        MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;
        MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnBannerAdExpandedEvent;
        MaxSdkCallbacks.Banner.OnAdCollapsedEvent += OnBannerAdCollapsedEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
    }
    public void RemoveListenerBannerAds()
    {
        MaxSdkCallbacks.Banner.OnAdLoadedEvent -= OnBannerAdLoadedEvent;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent -= OnBannerAdLoadFailedEvent;
        MaxSdkCallbacks.Banner.OnAdClickedEvent -= OnBannerAdClickedEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent -= OnBannerAdRevenuePaidEvent;
        MaxSdkCallbacks.Banner.OnAdExpandedEvent -= OnBannerAdExpandedEvent;
        MaxSdkCallbacks.Banner.OnAdCollapsedEvent -= OnBannerAdCollapsedEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent -= OnAdRevenuePaidEvent;
    }


    private void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        
    }

    private void OnBannerAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo) 
    {
        
    }

    private void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) 
    {
        
    }

    private void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnBannerAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnBannerAdCollapsedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    #endregion
    #region Interstitial
    public void InitializeInterstitialAds()
    {
        // Attach callback
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent_Interstitial;

    }


    public void RemoveListenerInterstitialAds()
    {
        // Attach callback
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent -= OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent -= OnInterstitialLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent -= OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent -= OnInterstitialClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent -= OnInterstitialHiddenEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent -= OnInterstitialAdFailedToDisplayEvent;
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent -= OnAdRevenuePaidEvent_Interstitial;
    }

    private void OnAdRevenuePaidEvent_Interstitial(string arg1, MaxSdkBase.AdInfo adInfo)
    {
        OnAdRevenuePaidEvent(arg1, adInfo);
        //FirebaseManager.Instance.LogEvent_interstitial_show(Manager.Instance.interPoint, AdsAction.ad_impression, adInfo.Revenue);
    }
    private void LoadInterstitial(string adUnitId)
    {
        MaxSdk.LoadInterstitial(adUnitId);
    }

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'
        Stopwatch sw = null;
        lock (timers)
        {
            if (timers.TryGetValue(adUnitId, out sw)) timers.Remove(adUnitId);
        }
        var elapsedMs = sw != null ? sw.Elapsed.TotalMilliseconds : -1;
        
        // Reset retry attempt
        retryAttemptInterstitial = 0;
    }

    private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Interstitial ad failed to load 
        Stopwatch sw = null;
        lock (timers)
        {
            if (timers.TryGetValue(adUnitId, out sw)) timers.Remove(adUnitId);
        }
        var elapsedMs = sw != null ? sw.Elapsed.TotalMilliseconds : -1;
        
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)

        retryAttemptInterstitial++;
        double retryDelay = Math.Pow(2, Math.Min(6, retryAttemptInterstitial));

        Invoke("LoadInterstitial", (float)retryDelay);
    }

    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Time.timeScale = 0;
        //FirebaseManager.Instance.LogEvent_interstitial_show(Manager.Instance.interPoint, AdsAction.ad_show, adInfo.Revenue);
    }

    private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
        LoadInterstitial(adUnitId);
    }

    private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) 
    {
      
    }

    private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        double ms = (DateTime.Now - startShowAds).TotalMilliseconds;
        Time.timeScale = 1;
        

        StartCoroutine(DelayCloseAds(() =>
        {
            // Interstitial ad is hidden. Pre-load the next ad.
            LoadInterstitial(adUnitId);
            if (this.acceptCallback)
            {
                this.onComplete?.Invoke();
            }
            this.onComplete = null;
            currentTime = Time.unscaledTime;
            StartCoroutine(DelayCallback(5f, () =>
            {
                adsIsShowing = false;
            }));
        }));

        lastAdCloseTime = Time.realtimeSinceStartup;
        UnpauseCheck();

    }
    #endregion
    #region Rewarded
    public void InitializeRewardedAds()
    {
        // Attach callback
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent_Rewarded;

    }
    public void RemoveListenerRewardedAds()
    {
        // Attach callback
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent -= OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent -= OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent -= OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent -= OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent -= OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent -= OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent -= OnRewardedAdReceivedRewardEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent -= OnAdRevenuePaidEvent_Rewarded;
    }
    private void OnAdRevenuePaidEvent_Rewarded(string arg1, MaxSdkBase.AdInfo adInfo)
    {
        OnAdRevenuePaidEvent(arg1, adInfo);
        //FirebaseManager.Instance.LogEvent_reward_show(Manager.Instance.rewardPoint, AdsAction.ad_impression, Manager.Instance.progress, adInfo.Revenue);

    }
    private void LoadRewardedAd(string adUnitId)
    {
        MaxSdk.LoadRewardedAd(adUnitId);
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.
        Stopwatch sw = null;
        lock (timers)
        {
            if (timers.TryGetValue(adUnitId, out sw)) timers.Remove(adUnitId);
        }
        var elapsedMs = sw != null ? sw.Elapsed.TotalMilliseconds : -1;
        
        // Reset retry attempt
        retryAttemptRewarded = 0;
    }

    private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to load 
        Stopwatch sw = null;
        lock (timers)
        {
            if (timers.TryGetValue(adUnitId, out sw)) timers.Remove(adUnitId);
        }
        var elapsedMs = sw != null ? sw.Elapsed.TotalMilliseconds : -1;
        
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this cas      Debug.LogError("InterRewardChecker 1");e 64 seconds).

        retryAttemptRewarded++;
        double retryDelay = Math.Pow(2, Math.Min(6, retryAttemptRewarded));

        Invoke("LoadRewardedAd", (float)retryDelay);
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Time.timeScale = 0;
        //FirebaseManager.Instance.LogEvent_reward_show(Manager.Instance.rewardPoint, AdsAction.ad_show, Manager.Instance.progress, adInfo.Revenue);
    }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        //Debug.LogError("ADS:  Reward fail to display");
        // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
        LoadRewardedAd(adUnitId);
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) 
    {
        
    }

    private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        double ms = (DateTime.Now - startShowAds).TotalMilliseconds; UnityEngine.Debug.Log("End Show Reward " + DateTime.Now + " " + startShowAds);
        Time.timeScale = 1;

        StartCoroutine(DelayCloseAds(() =>
        {
            // Rewarded ad is hidden. Pre-load the next ad
            LoadRewardedAd(adUnitId);
            if (this.acceptCallback)
            {
                this.onComplete?.Invoke();
            }
            this.onComplete = null;
            currentTime = Time.unscaledTime;
            StartCoroutine(DelayCallback(5f, () =>
            {
                adsIsShowing = false;
            }));

            //FirebaseManager.Instance.LogEvent_reward_show(Manager.Instance.rewardPoint, AdsAction.claim_success);
        }));

        lastAdCloseTime = Time.realtimeSinceStartup;
        UnpauseCheck();

    }
    private void PauseCheck()
    {
       
    }

    private Coroutine pauseCheckCoroutine;

    private void UnpauseCheck()
    {
        if (pauseCheckCoroutine != null)
        {
            StopCoroutine(pauseCheckCoroutine);
        }
        pauseCheckCoroutine = StartCoroutine(PauseCheckRoutine());

    }

    private IEnumerator PauseCheckRoutine()
    {
        yield return new WaitForSeconds(1);
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        // The rewarded ad displayed and the user should receive the reward.
        this.acceptCallback = true;
    }

    #endregion
    

    IEnumerator DelayCloseAds(Action callback)
    {
        yield return new WaitForEndOfFrame();
        callback?.Invoke();
    }

    IEnumerator DelayCallback(float time, Action callback)
    {
        yield return new WaitForSeconds(time);
        callback?.Invoke();
    }

    public bool IsShowPopupDefault(string adUnitId)
    {

#if !UNITY_EDITOR
        if (!MaxSdk.IsInterstitialReady(adUnitId))
        {
            return false;
        }
#endif
        float time = Time.unscaledTime - currentTime;
        currentTimeCheck = time;
        float timeShowInterstitial = 60f;
        if (time >= timeShowInterstitial)
        {
            currentTime = Time.unscaledTime;
            return true;
        }
        //Debug.LogError("Time Interstitial:" + time + "//" + timeShowInterstitial);
        return false;
    }

    public void ResetTimeShowPopup()
    {
        currentTime = Time.unscaledTime;
    }

    
    private void OnAdRevenuePaidEvent(string arg1, MaxSdkBase.AdInfo adInfo)
    {
        // TrackingRevenueConnector.SendRevenue_ToFirebase_MaxApplovin(adInfo);
        // TrackingRevenueConnector.SendRevenue_ToAppflyer_MaxApplovin(adInfo);
        // TrackingRevenueConnector.SendRevenue_ToFacebook_MaxApplovin(adInfo);
    }

}
