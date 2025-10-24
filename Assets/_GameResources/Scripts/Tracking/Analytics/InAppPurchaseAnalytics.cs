using Firebase.Analytics;
using System.Collections.Generic;
using UnityEngine;
using Yoolax.Framework;

namespace Analytics
{
    public static class InAppPurchaseAnalytics
    {
        public static void InitEvent()
        {
            Server.Get<OnIAPShowEventLog>().AddListener(LogIAPShowEvent);
            Server.Get<OnIAPClickEventLog>().AddListener(LogIAPClickEvent);
            Server.Get<OnIAPPurchaseEventLog>().AddListener(LogIAPPurchaseEvent);
            Server.Get<OnIAPCloseEventLog>().AddListener(LogIAPCloseEvent);
        }

        public static void LogIAPShowEvent(InAppPurchaseAnalyticStruct iapStruct)
        {
            if(AnalyticManager.Instance) AnalyticManager.Instance.timeOpenPopupIap = Time.time;
            var parameters = new[]
            {
            new Parameter("placement", iapStruct.placement),
            new Parameter("show_type", iapStruct.showType.ToString()),
            new Parameter("trigger_type", iapStruct.triggerType.ToString()),
            new Parameter("pack_name", iapStruct.pack_name),           
        };
            Dictionary<string, object> paramData = new Dictionary<string, object>
            {
                { "placement", iapStruct.placement },
                { "show_type", iapStruct.showType.ToString() },
                { "trigger_type", iapStruct.triggerType.ToString() },
                { "pack_name", iapStruct.pack_name }                
            };
            FirebaseManager.Instance.AddEvent("iap_show", paramData, parameters);
        }

        public static void LogIAPClickEvent(InAppPurchaseAnalyticStruct iapStruct)
        {
            var parameters = new[]
            {
            new Parameter("placement", iapStruct.placement),
            new Parameter("show_type", iapStruct.showType.ToString()),
            new Parameter("trigger_type", iapStruct.triggerType.ToString()),
            new Parameter("pack_name", iapStruct.pack_name),
        };

            Dictionary<string, object> paramData = new Dictionary<string, object>
            {
                { "placement", iapStruct.placement },
                { "show_type", iapStruct.showType.ToString() },
                { "trigger_type", iapStruct.triggerType.ToString() },
                { "pack_name", iapStruct.pack_name }
            };
            FirebaseManager.Instance.AddEvent("iap_click", paramData, parameters);
        }

        public static void LogIAPPurchaseEvent(InAppPurchaseAnalyticStruct iapStruct)
        {
            var parameters = new[]
            {
            new Parameter("placement", iapStruct.placement),
            new Parameter("show_type", iapStruct.showType.ToString()),
            new Parameter("trigger_type", iapStruct.triggerType.ToString()),
            new Parameter("pack_name", iapStruct.pack_name),
            new Parameter("price", iapStruct.price),
            new Parameter("currency", iapStruct.currency)
        };

            Dictionary<string, object> paramData = new Dictionary<string, object>
            {
                { "placement", iapStruct.placement },
                { "show_type", iapStruct.showType.ToString() },
                { "trigger_type", iapStruct.triggerType.ToString() },
                { "pack_name", iapStruct.pack_name },
                { "price", iapStruct.price },
                { "currency", iapStruct.currency }
            };
            FirebaseManager.Instance.AddEvent("iap_purchase", paramData, parameters);
        }

        public static void LogIAPCloseEvent(InAppPurchaseAnalyticStruct iapStruct)
        {
            var parameters = new[]
            {
            new Parameter("placement", iapStruct.placement),
            new Parameter("show_type", iapStruct.showType.ToString()),
            new Parameter("trigger_type", iapStruct.triggerType.ToString()),
            new Parameter("pack_name", iapStruct.pack_name),
            new Parameter("view_time", iapStruct.view_time)          
        };

            Dictionary<string, object> paramData = new Dictionary<string, object>
            {
                { "placement", iapStruct.placement },
                { "show_type", iapStruct.showType.ToString() },
                { "trigger_type", iapStruct.triggerType.ToString() },
                { "pack_name", iapStruct.pack_name },
                { "view_time", iapStruct.view_time }                
            };
            FirebaseManager.Instance.AddEvent("iap_close", paramData, parameters);
        }
    }

    public struct InAppPurchaseAnalyticStruct
    {
        public string placement;
        public IAPShowType showType;
        public TriggerType triggerType;
        public string pack_name;
        public string reason;
        public float price;
        public string currency;
        public float view_time;

        public InAppPurchaseAnalyticStruct SetBaseIAP(string placement, IAPShowType showType, TriggerType triggerType, string packName)
        {
            this.placement =  placement;
            this.showType = showType;
            this.triggerType = triggerType;
            this.pack_name = packName;           
            return this;
        }

        public InAppPurchaseAnalyticStruct SetIAPShow()
        {            
            return this;
        }

        public InAppPurchaseAnalyticStruct SetIAPClick()
        {
            return this;
        }

        public InAppPurchaseAnalyticStruct SetIAPPurchase(float price, string currency)
        {
            this.price = price;
            this.currency = currency;
            return this;
        }

        public InAppPurchaseAnalyticStruct SetIAPClose(float viewTime)
        {
            this.view_time = viewTime;
            return this;
        }
    }

    public enum IAPShowType
    {
        shop,
        pack
    }

    public enum TriggerType
    {
        popup,
        click
    }
}

