using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yoolax.Framework;

namespace Analytics
{
    [DefaultExecutionOrder(-101)]
    public class AnalyticManager : SingletonDontDestroyMono<AnalyticManager>
    {
        public float timeOpenPopupIap;
        public float timeOpenFeature;
        public string adPlacement = "unknown";
        public TriggerType iAPTriggerType;
        public IAPShowType iAPShow;

        protected override void Awake()
        {
            base.Awake();
            InitAnalyticEvent();
        }

        public float GetTimeInFeatureMsec()
        {
            return (Time.time - timeOpenFeature) * 1000f;
        }

        public string GetCurrentPrefixPlacement()
        {
            if (SceneManager.GetActiveScene().name == "MainMenu")
            {
                return AnalyticUtils.prefixPlacementHome;
            }

            return AnalyticUtils.prefixPlacementGameplay;
        }

        public void InitAnalyticEvent()
        {
            UserProperties.InitEvent();
            LevelAnalytics.InitEvent();
            ResourceAnalytics.InitEvent();
            InAppPurchaseAnalytics.InitEvent();
            AdsAnalytics.InitEvent();
            FeatureAnalytics.InitEvent();
            OtherMetricsAnalytics.InitEvent();
        }
    }
}

