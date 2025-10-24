using Firebase.Analytics;
using System.Collections.Generic;
using Yoolax.Framework;

namespace Analytics
{
    public static class AdsAnalytics
    {
        public static void InitEvent()
        {
            Server.Get<OnAdRequestEventLog>().AddListener(LogAdRequestEvent);
            Server.Get<OnAdClickEventLog>().AddListener(LogAdClickEvent);
            Server.Get<OnAdImpressionEventLog>().AddListener(LogAdImpressionEvent);
            Server.Get<OnAdCompleteEventLog>().AddListener(LogAdCompleteEvent);
        }

        public static void LogAdRequestEvent(AdsAnalyticStruct adStruct)
        {
            var parameters = new[]
            {
            new Parameter("ad_format", adStruct.adFormat),
            new Parameter("ad_platform", adStruct.adPlatform),
            new Parameter("ad_network", adStruct.adNetwork),
            new Parameter("placement", adStruct.placement),
            new Parameter("is_load", adStruct.isLoad),
            new Parameter("load_time", adStruct.loadTime),
        };

            Dictionary<string, object> paramData = new Dictionary<string, object>
            {
                { "ad_format", adStruct.adFormat },
                { "ad_platform", adStruct.adPlatform },
                { "ad_network", adStruct.adNetwork },
                { "placement", adStruct.placement },
                { "is_load", adStruct.isLoad },
                { "load_time", adStruct.loadTime }
            };
            FirebaseManager.Instance.AddEvent("ad_request", paramData, parameters);
        }

        public static void LogAdImpressionEvent(AdsAnalyticStruct adStruct)
        {
            var parameters = new[]
            {
            new Parameter("ad_format", adStruct.adFormat),
            new Parameter("ad_platform", adStruct.adPlatform),
            new Parameter("ad_network", adStruct.adNetwork),
            new Parameter("placement", adStruct.placement),
            new Parameter("is_show", adStruct.isShow),
            new Parameter("value", adStruct.value),
        };
            Dictionary<string, object> paramData = new Dictionary<string, object>
            {
                { "ad_format", adStruct.adFormat },
                { "ad_platform", adStruct.adPlatform },
                { "ad_network", adStruct.adNetwork },
                { "placement", adStruct.placement },
                { "is_show", adStruct.isShow },
                { "value", adStruct.value }
            };
            FirebaseManager.Instance.AddEvent("ad_impression", paramData, parameters);
        }

        public static void LogAdClickEvent(AdsAnalyticStruct adStruct)
        {
            var parameters = new[]
            {
            new Parameter("ad_format", adStruct.adFormat),
            new Parameter("ad_platform", adStruct.adPlatform),
            new Parameter("ad_network", adStruct.adNetwork),
            new Parameter("placement", adStruct.placement)           
        };
            Dictionary<string, object> paramData = new Dictionary<string, object>
            {
                { "ad_format", adStruct.adFormat },
                { "ad_platform", adStruct.adPlatform },
                { "ad_network", adStruct.adNetwork },
                { "placement", adStruct.placement }               
            };
            FirebaseManager.Instance.AddEvent("ad_click", paramData, parameters);
        }

        public static void LogAdCompleteEvent(AdsAnalyticStruct adStruct)
        {
            var parameters = new[]
            {
            new Parameter("ad_format", adStruct.adFormat),
            new Parameter("ad_platform", adStruct.adPlatform),
            new Parameter("ad_network", adStruct.adNetwork),
            new Parameter("end_type", adStruct.endType.ToString()),
            new Parameter("ad_duration", adStruct.adDuration),
            new Parameter("placement", adStruct.placement)
        };
            Dictionary<string, object> paramData = new Dictionary<string, object>
            {
                { "ad_format", adStruct.adFormat },
                { "ad_platform", adStruct.adPlatform },
                { "ad_network", adStruct.adNetwork },
                { "end_type", adStruct.endType.ToString() },
                { "ad_duration", adStruct.adDuration },
                { "placement", adStruct.placement }
            };
            FirebaseManager.Instance.AddEvent("ad_complete", paramData, parameters);
        }
    }

    public struct AdsAnalyticStruct
    {
        public string adFormat;
        public string adPlatform;
        public string adNetwork;
        public string placement;
        public int isLoad;
        public float loadTime;
        public int isShow;
        public double value;
        public EndType endType;
        public float adDuration;

        public AdsAnalyticStruct SetBaseAd(string adFormat, string adPlatform, string adNetwork, string placement)
        {
            this.placement = placement;
            this.adFormat = adFormat;
            this.adPlatform = adPlatform;
            this.adNetwork = adNetwork;
            return this;
        }

        public AdsAnalyticStruct SetAdRequest(int isLoad, float loadTime)
        {
            this.isLoad = isLoad;
            this.loadTime = loadTime;           
            return this;
        }

        public AdsAnalyticStruct SetAdImpression(int isShow, double value)
        {
            this.isShow = isShow;
            this.value = value;
            return this;
        }

        public AdsAnalyticStruct SetAdClick()
        {            
            return this;
        }

        public AdsAnalyticStruct SetAdComplete(EndType endType, float adDuration)
        {
            this.endType = endType;
            this.adDuration = adDuration;
            return this;
        }
    }

    public enum EndType
    {
        quit,
        done
    }
}

