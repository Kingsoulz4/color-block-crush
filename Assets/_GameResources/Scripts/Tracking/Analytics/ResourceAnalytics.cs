using Yoolax.Framework;
using UnityEngine;
using Firebase.Analytics;
using System.Collections.Generic;

namespace Analytics
{
    public static class ResourceAnalytics
    {
        public static void InitEvent()
        {
            Server.Get<OnResourceEarnEventLog>().AddListener(LogResourceEarnEvent);
            Server.Get<OnResourceSpendEventLog>().AddListener(LogResourceSpendEvent);
        }

        public static void LogResourceEarnEvent(ResourceAnalyticStruct resourceStruct)
        {
#if UNITY_FIREBASE
            var parameters = new[]
            {
            new Parameter("resource_type", resourceStruct.resourceType),
            new Parameter("resource_name", resourceStruct.resourceName),
            new Parameter("resource_amount", resourceStruct.resourceAmount),
            new Parameter("placement", resourceStruct.placement),
            new Parameter("reason", resourceStruct.reason),            
        };
            Dictionary<string, object> paramData = new Dictionary<string, object>
            {
                { "resource_type", resourceStruct.resourceType },
                { "resource_name", resourceStruct.resourceName },
                { "resource_amount", resourceStruct.resourceAmount },
                { "placement", resourceStruct.placement },
                { "reason", resourceStruct.reason}
            };
            FirebaseManager.Instance.AddEvent("resource_earn", paramData, parameters);
#endif
        }

        public static void LogResourceSpendEvent(ResourceAnalyticStruct resourceStruct)
        {
#if UNITY_FIREBASE
            var parameters = new[]
            {
            new Parameter("resource_type", resourceStruct.resourceType),
            new Parameter("resource_name", resourceStruct.resourceName),
            new Parameter("resource_amount", resourceStruct.resourceAmount),
            new Parameter("placement", resourceStruct.placement),
            new Parameter("reason", resourceStruct.reason),
        };
            Dictionary<string, object> paramData = new Dictionary<string, object>
            {
                { "resource_type", resourceStruct.resourceType },
                { "resource_name", resourceStruct.resourceName },
                { "resource_amount", resourceStruct.resourceAmount },
                { "placement", resourceStruct.placement },
                { "reason", resourceStruct.reason}
            };
            FirebaseManager.Instance.AddEvent("resource_spend", paramData, parameters);
#endif
        }
    }

    public struct ResourceAnalyticStruct
    {
        public string resourceType;
        public string resourceName;
        public string resourceAmount;
        public string placement;
        public string reason;

        public ResourceAnalyticStruct(string[] types, string[] names, string[] amounts, string placements, string reason)
        {
            this.resourceType = AnalyticUtils.ConnectString(types);
            this.resourceName = AnalyticUtils.ConnectString(names); Debug.Log("Connect Complete " + this.resourceName);
            this.resourceAmount = AnalyticUtils.ConnectString(amounts);
            this.placement = placements;
            this.reason = reason;
        }
    }

    public struct ResourceInfo
    {
        public string resourceType;
        public string resourceName;
        public string resourceAmount;
    }

    public enum ResourceType
    {
        currency,
        item,
        booster
    }

    public enum ReasonType
    {
        reward,
        exchange,
        purchase,
        watch_ads,
        time,
        use
    }
}
