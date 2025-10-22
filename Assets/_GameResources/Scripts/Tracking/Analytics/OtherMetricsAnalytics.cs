using Firebase.Analytics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yoolax.Framework;

namespace Analytics
{
    public static class OtherMetricsAnalytics
    {
        public static void InitEvent()
        {
            Server.Get<OnButtonClickEventLog>().AddListener(LogButtonClickEvent);
            Server.Get<OnLoadingSceneStartEventLog>().AddListener(LogLoadingSceneStartEvent);
            Server.Get<OnLoadingSceneEndEventLog>().AddListener(LogLoadingSceneEndEvent);
        }

        public static void LogButtonClickEvent(ButtonAnalyticStruct buttonClickStruct)
        {
#if UNITY_FIREBASE
            var parameters = new[]
            {
            new Parameter("button_name", buttonClickStruct.button_name),
            new Parameter("screen_name", buttonClickStruct.screen_name),            
        };

            Dictionary<string, object> paramData = new Dictionary<string, object>
            {
                { "button_name", buttonClickStruct.button_name },
                { "screen_name", buttonClickStruct.screen_name }                
            };
            FirebaseManager.Instance.AddEvent("button_click", paramData, parameters);
#endif
        }

        public static void LogLoadingSceneStartEvent(LoadingScneneAnalyticStruct loadingSceneStruct)
        {
#if UNITY_FIREBASE
            var parameters = new[]
            {
            new Parameter("placement", loadingSceneStruct.placement)            
        };

            Dictionary<string, object> paramData = new Dictionary<string, object>
            {
                { "placement", loadingSceneStruct.placement }                
            };
            FirebaseManager.Instance.AddEvent("loading_start", paramData, parameters);
#endif
        }

        public static void LogLoadingSceneEndEvent(LoadingScneneAnalyticStruct loadingSceneStruct)
        {
#if UNITY_FIREBASE
            var parameters = new[]
            {
            new Parameter("placement", loadingSceneStruct.placement),
            new Parameter("is_load", loadingSceneStruct.is_load),
            new Parameter("load_time", loadingSceneStruct.load_time),
        };

            Dictionary<string, object> paramData = new Dictionary<string, object>
            {
                { "placement", loadingSceneStruct.placement },
                { "is_load", loadingSceneStruct.is_load },
                {"load_time", loadingSceneStruct.load_time }
            };
            FirebaseManager.Instance.AddEvent("loading_finish", paramData, parameters);
#endif
        }
    }

    public struct ButtonAnalyticStruct
    {
        public string button_name;
        public string screen_name;

        public ButtonAnalyticStruct(string button_name, string screen_name)
        {
            this.button_name = button_name;
            this.screen_name = screen_name;
        }
    }

    public struct LoadingScneneAnalyticStruct
    {
        public string placement;
        public int is_load;
        public float load_time;

        public LoadingScneneAnalyticStruct SetLoadingSceneBase(string placement)
        {
            this.placement = placement;
            return this;
        }

        public LoadingScneneAnalyticStruct SetLoadingSceneStart()
        {
            return this;
        }

        public LoadingScneneAnalyticStruct SetLoadingSceneEnd(int is_load, float load_time)
        {
            this.is_load = is_load;
            this.load_time = load_time;
            return this;
        }
    }
}

