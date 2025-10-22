using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Analytics
{
    public class DatabucketsSender : MonoBehaviour
    {
        public static DatabucketsSender I { get; private set; }

        public static string GetSessionId()
        {
            return PlayerPrefs.GetString("analytics_session_id", null) ?? CreateNewSession();
        }

        static string CreateNewSession()
        {
            string s = Guid.NewGuid().ToString();
            PlayerPrefs.SetString("analytics_session_id", s);
            PlayerPrefs.Save();
            return s;
        }

        // Configurable
        [Header("Databuckets")]
        [SerializeField] string url = "https://ingest.databuckets.com/push";
        [SerializeField] string apiKey = "";
        [Header("Batching")]
        [SerializeField] int batchCountThreshold = 50;
        [SerializeField] int batchBytesThreshold = 512 * 1024; // 512 KB
        [SerializeField] float flushInterval = 15f; // seconds
        [SerializeField] int maxRetries = 5;

        // Internal
        Queue<string> queue = new Queue<string>();
        string persistPath;
        Coroutine flushLoop;
        bool isFlushing = false;

        void Awake()
        {
            if (I != null && I != this) { Destroy(gameObject); return; }
            I = this;
            DontDestroyOnLoad(gameObject);
            persistPath = Path.Combine(Application.persistentDataPath, "databuckets_queue.log");
            LoadQueueFromDisk();
        }

        void Start()
        {
            //flushLoop = StartCoroutine(PeriodicFlush());
        }

        void OnApplicationPause(bool paused)
        {
            if (paused)
            {
                // app going to background: persist and try flush
                //SaveQueueToDisk();
                //TryFlushImmediate();
            }
        }

        void OnApplicationQuit()
        {
            // persist on quit (best-effort)
            //SaveQueueToDisk();
        }        

        bool ShouldFlushByThreshold()
        {
            lock (queue)
            {
                if (queue.Count >= batchCountThreshold) return true;
                int approxBytes = 0;
                foreach (var s in queue) approxBytes += Encoding.UTF8.GetByteCount(s) + 1; // newline
                return approxBytes >= batchBytesThreshold;
            }
        }              

        public void SendEvent(Dictionary<string, object> evt)
        {
            UserProperties.AddPropertyToEvent(evt);
            string jsonLine = DictToJsonOneLine(evt);
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.LogWarning($"No Internet To Push");
                queue.Enqueue(jsonLine);
                SaveQueueToDisk();
            }
            else
            {
                StartCoroutine(SendEventsCoroutine(jsonLine));
            }            
        }

        public IEnumerator SendEventsCoroutine(string payload)
        {            
            byte[] bodyRaw = Encoding.UTF8.GetBytes(payload);
            using (UnityWebRequest req = new UnityWebRequest(url, "POST"))
            {
                req.uploadHandler = new UploadHandlerRaw(bodyRaw);
                req.downloadHandler = new DownloadHandlerBuffer();
                req.SetRequestHeader("Content-Type", "application/json");
                req.SetRequestHeader("X-API-KEY", apiKey);
                yield return req.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
                if (req.result != UnityWebRequest.Result.Success)
#else
            if (req.isNetworkError || req.isHttpError)
#endif
                {
                    Debug.LogError("Databuckets send failed: " + req.error);
                }
                else
                {
                    SaveQueueToDisk();
                    Debug.Log("Databuckets response: " + req.downloadHandler.text);
                }
            }
        }        

        void SaveQueueToDisk()
        {
            Debug.Log("Save Queue To Disk");
            try
            {
                File.WriteAllText(persistPath, string.Join("\n", queue));
            }
            catch (Exception e)
            {
                Debug.LogWarning("SaveQueueToDisk failed: " + e.Message);
            }
        }

        void LoadQueueFromDisk()
        {
            try
            {
                if (!File.Exists(persistPath)) return;
                var text = File.ReadAllText(persistPath);
                if (string.IsNullOrWhiteSpace(text)) return;
                var lines = text.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                queue = new Queue<string>(lines);
                foreach (var line in lines)
                    queue.Enqueue(line);
            }
            catch (Exception e) { Debug.LogWarning("LoadQueueFromDisk failed: " + e.Message); }

            Debug.Log($"Try Push Queue Event {queue.Count}");
            StartCoroutine(TryPushQueueEventToDataBucket());
        }

        IEnumerator TryPushQueueEventToDataBucket()
        {
            if (queue.Count > 0 && Application.internetReachability != NetworkReachability.NotReachable)
            {
                while (queue.Count > 0)
                {
                    string json = queue.Dequeue();
                    yield return StartCoroutine(SendEventsCoroutine(json));
                }
            }
        }

        string DictToJsonOneLine(Dictionary<string, object> dict)
        {
            var sb = new StringBuilder();
            sb.Append('{');
            bool first = true;
            foreach (var kv in dict)
            {
                if (!first) sb.Append(',');
                first = false;
                sb.Append('\"').Append(EscapeJson(kv.Key)).Append("\":");
                if (kv.Value == null)
                {
                    sb.Append("null");
                }
                else if (kv.Value is string)
                {
                    sb.Append('\"').Append(EscapeJson(kv.Value as string)).Append('\"');
                }
                else if (kv.Value is bool)
                {
                    sb.Append((bool)kv.Value ? "true" : "false");
                }
                else if (kv.Value is int || kv.Value is long || kv.Value is float || kv.Value is double || kv.Value is decimal)
                {
                    // write numeric as-is
                    sb.Append(Convert.ToString(kv.Value, System.Globalization.CultureInfo.InvariantCulture));
                }
                else
                {
                    // fallback to string representation
                    sb.Append('\"').Append(EscapeJson(kv.Value.ToString())).Append('\"');
                }
            }
            sb.Append('}');
            return sb.ToString();
        }

        string EscapeJson(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            var sb = new StringBuilder();
            foreach (char c in s)
            {
                switch (c)
                {
                    case '\"': sb.Append("\\\""); break;
                    case '\\': sb.Append("\\\\"); break;
                    case '\b': sb.Append("\\b"); break;
                    case '\f': sb.Append("\\f"); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\t': sb.Append("\\t"); break;
                    default:
                        if (c < 32 || c > 126) sb.AppendFormat("\\u{0:X4}", (int)c);
                        else sb.Append(c);
                        break;
                }
            }
            return sb.ToString();
        }       
    }
}

