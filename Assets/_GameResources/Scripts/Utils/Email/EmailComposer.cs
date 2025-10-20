using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ColorBlockCrush
{
    public static class EmailComposer
    {
        public static void ComposeEmail(
        IEnumerable<string> to,
        IEnumerable<string> cc = null,
        IEnumerable<string> bcc = null,
        string subject = null,
        string body = null,
        string androidChooserTitle = "Choose email app")
    {
        // Sanity
        if (to == null || !to.Any())
            throw new ArgumentException("At least 1 recipient email (To) is required.");

#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (var intent = new AndroidJavaObject("android.content.Intent", "android.intent.action.SENDTO"))
            using (var uriClass = new AndroidJavaClass("android.net.Uri"))
            {
                var mailto = uriClass.CallStatic<AndroidJavaObject>("parse", "mailto:");
                intent.Call<AndroidJavaObject>("setData", mailto);

                if (to != null)  intent.Call<AndroidJavaObject>("putExtra", "android.intent.extra.EMAIL", to.ToArray());
                if (cc != null)  intent.Call<AndroidJavaObject>("putExtra", "android.intent.extra.CC",    cc.ToArray());
                if (bcc != null) intent.Call<AndroidJavaObject>("putExtra", "android.intent.extra.BCC",   bcc.ToArray());
                if (!string.IsNullOrEmpty(subject)) intent.Call<AndroidJavaObject>("putExtra", "android.intent.extra.SUBJECT", subject);
                if (!string.IsNullOrEmpty(body))    intent.Call<AndroidJavaObject>("putExtra", "android.intent.extra.TEXT",    body);

                using (var intentClass = new AndroidJavaClass("android.content.Intent"))
                using (var chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intent, androidChooserTitle))
                {
                    activity.Call("startActivity", chooser);
                }
            }
            return;
        }
        catch (AndroidJavaException)
        {
            Application.OpenURL(BuildMailtoUrl(to, cc, bcc, subject, body));
            return;
        }
#elif UNITY_IOS && !UNITY_EDITOR
        Application.OpenURL(BuildMailtoUrl(to, cc, bcc, subject, body));
        return;
#else
        Application.OpenURL(BuildMailtoUrl(to, cc, bcc, subject, body));
        return;
#endif
    }
        
    private static string BuildMailtoUrl(IEnumerable<string> to, IEnumerable<string> cc, IEnumerable<string> bcc, string subject, string body)
    {
        string toPart = JoinEmails(to);
        var query = new List<string>();

        if (!string.IsNullOrEmpty(subject)) query.Add("subject=" + Uri.EscapeDataString(subject));
        if (!string.IsNullOrEmpty(body))    query.Add("body="    + Uri.EscapeDataString(body));
        if (cc  != null && cc.Any())        query.Add("cc="      + Uri.EscapeDataString(JoinEmails(cc)));
        if (bcc != null && bcc.Any())       query.Add("bcc="     + Uri.EscapeDataString(JoinEmails(bcc)));

        string q = query.Count > 0 ? "?" + string.Join("&", query) : "";
        return "mailto:" + toPart + q;
    }

    private static string JoinEmails(IEnumerable<string> emails)
        => emails == null ? "" : string.Join(",", emails.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()));
    }
}
