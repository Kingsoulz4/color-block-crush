using System.Collections;
using System.Collections.Generic;
using ColorBlockCrush;
using UnityEngine;
public enum ToastDuration
{
	Short,
	Long
}
public class ToastUtil
{
	public static void ShowToast(string message, ToastDuration duration = ToastDuration.Short)
	{       
		if (!TestManager.IsCheating)
			return;
#if UNITY_ANDROID && !UNITY_EDITOR
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            activity.Call("runOnUiThread", new AndroidJavaRunnable(() => 
            {
                AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");

                int toastDuration = duration == ToastDuration.Short 
                                    ? toastClass.GetStatic<int>("LENGTH_SHORT") 
                                    : toastClass.GetStatic<int>("LENGTH_LONG");

                AndroidJavaObject toast = toastClass.CallStatic<AndroidJavaObject>("makeText", context, message, toastDuration);
                toast.Call("show");
            }));
        }
#else
        Debug.Log("Toast: " + message);
#endif


    }

	public static void ShowToastDebug(string message, ToastDuration duration = ToastDuration.Short)
	{
		if (!TestManager.IsCheating)
			return;

#if UNITY_ANDROID && !UNITY_EDITOR
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            activity.Call("runOnUiThread", new AndroidJavaRunnable(() => 
            {
                AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");

                int toastDuration = duration == ToastDuration.Short 
                                    ? toastClass.GetStatic<int>("LENGTH_SHORT") 
                                    : toastClass.GetStatic<int>("LENGTH_LONG");

                AndroidJavaObject toast = toastClass.CallStatic<AndroidJavaObject>("makeText", context, message, toastDuration);
                toast.Call("show");
            }));
        }
#else
		Debug.Log("Toast: " + message);
#endif


	}
}

