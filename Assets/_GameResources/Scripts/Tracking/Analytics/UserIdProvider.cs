using System;
using System.Text;
using UnityEngine;
using System.Security.Cryptography;

public static class UserIdProvider
{
    const string PLAYERPREFS_KEY = "app_user_id_v1";

    public static string GetUserId()
    {
        try
        {           
            string stored = PlayerPrefs.GetString(PLAYERPREFS_KEY, null);
            if (!string.IsNullOrEmpty(stored)) return stored;
            
            string deviceId = SystemInfo.deviceUniqueIdentifier;
            if (!string.IsNullOrEmpty(deviceId) && !IsBadIdentifier(deviceId))
            {
                SaveId(deviceId);
                return deviceId;
            }
          
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                using (var up = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    var currentActivity = up.GetStatic<AndroidJavaObject>("currentActivity");
                    var contentResolver = currentActivity.Call<AndroidJavaObject>("getContentResolver");
                    using (var secure = new AndroidJavaClass("android.provider.Settings$Secure"))
                    {
                        string androidId = secure.CallStatic<string>("getString", contentResolver, "android_id");
                        if (!string.IsNullOrEmpty(androidId))
                        {
                            SaveId(androidId);
                            return androidId;
                        }
                    }
                }
            }
            catch (Exception) { /* ignore */ }
#endif
            
            string newId = Guid.NewGuid().ToString();
            SaveId(newId);
            return newId;
        }
        catch (Exception e)
        {
            Debug.LogWarning("UserIdProvider.GetUserId failed: " + e.Message);
            // last resort
            string fallback = Guid.NewGuid().ToString();
            SaveId(fallback);
            return fallback;
        }
    }

    static bool IsBadIdentifier(string id)
    {
        if (string.IsNullOrEmpty(id)) return true;
        // Unity sometimes returns same placeholder values on some platforms, filter them
        string low = id.ToLowerInvariant();
        if (low.Contains("unknown") || low.Contains("0000") || low.Contains("null")) return true;
        return false;
    }

    static void SaveId(string id)
    {
        try
        {
            PlayerPrefs.SetString(PLAYERPREFS_KEY, id);
            PlayerPrefs.Save();
        }
        catch (Exception e)
        {
            Debug.LogWarning("UserIdProvider.SaveId failed: " + e.Message);
        }
    }
    
    public static string GetUserIdHashed()
    {
        string id = GetUserId();
        return Sha256(id);
    }

    static string Sha256(string raw)
    {
        if (string.IsNullOrEmpty(raw)) return "";
        try
        {
            using (var sha = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(raw);
                byte[] hash = sha.ComputeHash(bytes);
                var sb = new StringBuilder();
                foreach (var b in hash) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("UserIdProvider.Sha256 failed: " + e.Message);
            return raw;
        }
    }
    
    public static void ResetUserId()
    {
        PlayerPrefs.DeleteKey(PLAYERPREFS_KEY);
        PlayerPrefs.Save();
    }
}
