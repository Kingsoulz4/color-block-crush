using Firebase.Analytics;
using System.Collections.Generic;
using UnityEngine;
using Yoolax.Framework;

namespace Analytics
{
    public static class FeatureAnalytics
    {
        public static void InitEvent()
        {
            Server.Get<OnEventFeatureFirstShow>().AddListener(LogEventFeatureFirstShow);
            Server.Get<OnEventFeatureOpen>().AddListener(LogEventFeatureOpen);
            Server.Get<OnEventFeatureClose>().AddListener(LogEventFeatureClose);            
        }

        public static void LogEventFeatureFirstShow(FeatureAnalyticStruct eventStruct)
        {
#if UNITY_FIREBASE
            var parameters = new[]
            {
            new Parameter("feature_name", eventStruct.featureName),
            new Parameter("placement", eventStruct.placement),            
        };
            Dictionary<string, object> paramData = new Dictionary<string, object>
            {
                { "feature_name", eventStruct.featureName },
                { "placement", eventStruct.placement }                  
            };
            FirebaseManager.Instance.AddEvent("feature_first_show", paramData, parameters);
#endif
        }

        public static void LogEventFeatureOpen(FeatureAnalyticStruct eventStruct)
        {
            if (AnalyticManager.Instance) AnalyticManager.Instance.timeOpenFeature = Time.time;
#if UNITY_FIREBASE
            var parameters = new[]
            {
            new Parameter("feature_name", eventStruct.featureName),
            new Parameter("placement", eventStruct.placement),
            new Parameter("open_type", eventStruct.openType.ToString()),
            new Parameter("open_index", eventStruct.openIndex)           
        };
            Dictionary<string, object> paramData = new Dictionary<string, object>
            {
                { "feature_name", eventStruct.featureName },
                { "placement", eventStruct.placement },
                { "open_type", eventStruct.openType.ToString() },
                { "open_index", eventStruct.openIndex }               
            };
            FirebaseManager.Instance.AddEvent("feature_open", paramData, parameters);
#endif
        }

        public static void LogEventFeatureClose(FeatureAnalyticStruct eventStruct)
        {
#if UNITY_FIREBASE
            var parameters = new[]
            {
            new Parameter("feature_name", eventStruct.featureName),
            new Parameter("placement", eventStruct.placement),
            new Parameter("open_index", eventStruct.openIndex),
            new Parameter("feature_duration", eventStruct.featureDuration),           
        };
            Dictionary<string, object> paramData = new Dictionary<string, object>
            {
                { "feature_name", eventStruct.featureName },
                { "placement", eventStruct.placement },
                { "open_index", eventStruct.openIndex },
                { "feature_duration", eventStruct.featureDuration }       
            };
            FirebaseManager.Instance.AddEvent("feature_close", paramData, parameters);
#endif
        }
    }

    public struct FeatureAnalyticStruct
    {
        public string featureName;
        public string placement;        
        public TriggerType openType;
        public int openIndex;           
        public float featureDuration;

        public FeatureAnalyticStruct SetBaseEvent(string eventName, string placement)
        {
            this.featureName = eventName;
            this.placement = placement;            
            return this;
        }

        public FeatureAnalyticStruct SetFeatureFirstShow()
        {           
            return this;
        }

        public FeatureAnalyticStruct SetFeatureOpen(TriggerType openType, int openIndex)
        {
            this.openType = openType;           
            this.openIndex = openIndex;
            return this;
        }

        public FeatureAnalyticStruct SetFeatureClose(int openIndex, float featureDuration)
        {
            this.openIndex = openIndex;
            this.featureDuration = featureDuration;
            return this;
        }      
    }

    public enum EventUnlockType
    {
        free,
        ads,
        iap
    }
}

