using Firebase.Analytics;
using System;
using System.Collections.Generic;
using UnityEngine;
using Analytics;
using Yoolax.Framework;

public enum EventDataType
{
	Property, NormalEvent, ParameterEvent
}

public abstract class EventData
{
	public abstract EventDataType Type { get; }

	public abstract void FireEvent();

}

public class PropertyEventData : EventData
{
	public string PropertyName { get; private set; }
	public string Value { get; private set; }
	public override EventDataType Type => EventDataType.Property;

    public PropertyEventData(string propertyName, string value)
    {
        this.PropertyName = propertyName;
		this.Value = value;
    }

	public override void FireEvent()
	{
		FirebaseAnalytics.SetUserProperty(this.PropertyName, this.Value);
#if UNITY_EDITOR
		Debug.Log("[EVENT PROPERTY] " + this.PropertyName + " - " + this.Value);
#endif

	}
}

public class NormalEventData: EventData
{
	public string EventName { get; private set; }

	public override EventDataType Type => EventDataType.NormalEvent;
	public NormalEventData(string eventName)
	{
		this.EventName = eventName;	
	}

	public override void FireEvent()
	{
		FirebaseAnalytics.LogEvent(EventName);

#if UNITY_EDITOR
		Debug.Log("[EVENT] " + EventName);
#endif
	}
}

public class ParatermterEventData : EventData {
	public override EventDataType Type => EventDataType.ParameterEvent;

	public string EventName { get; private set; }
	public Parameter[] paratermterEventDatas { get; private set; }
    public Dictionary<string, object> paramData;

    public ParatermterEventData(string eventName, Dictionary<string, object> paramData, params Parameter[] parameters)
	{
		this.EventName = eventName;
		this.paratermterEventDatas = parameters;	
		this.paramData = paramData;
	}

	public override void FireEvent()
	{
		FirebaseAnalytics.LogEvent(EventName, paratermterEventDatas);
		Server.Get<OnUserPropertiesLog>().Dispatch(AnalyticUtils.GetCurrentUserProperty());
        //try
        //{
        //    var evt = new Dictionary<string, object>();
        //    // required timestamp in epoch ms
        //    evt["_ts"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        //    evt["event_name"] = EventName;
        //    evt["user_id"] = UserIdProvider.GetUserIdHashed();
        //    evt["session_id"] = DatabucketsSender.GetSessionId();
        //    evt["app_id"] = Application.identifier;
        //    evt["app_version"] = Application.version;
        //    // copy parameters			
        //    if (paramData != null)
        //    {
        //        foreach (var kv in paramData)
        //        {
        //            // avoid collision with reserved keys
        //            if (kv.Value == "_ts" || kv.Key == "event_name") continue;
        //            evt[kv.Key] = kv.Value;
        //            Debug.Log("Key " + kv.Key + " Value " + kv.Value);
        //        }
        //    }

        //    DatabucketsSender.I?.SendEvent(evt);
        //}
        //catch (Exception e)
        //{
        //    Debug.LogWarning($"AnalyticsManager: enqueue to Databuckets failed: {e.Message}");
        //}

#if UNITY_EDITOR
        string parContent = string.Empty;
		Debug.Log("[EVENT PARA] " + EventName );
#endif
	}
}


