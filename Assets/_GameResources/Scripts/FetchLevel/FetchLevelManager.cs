using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using Yoolax.Framework;
using System.IO.Compression;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Buffers.Text;
using System.Linq;
using System.Threading.Tasks;
using ColorBlockCrush;
using ColorBlockCrush.Tools;
using DG.Tweening;

public class FetchLevelManager : SingletonDontDestroyMono<FetchLevelManager>
{
    public const string PRODUCT_URL = "https://openai.amobear.com/gamedata/ColorPixel/";
    public const string TEST_URL = "https://openai.amobear.com/gamedata/ColorPixelTest/";

    public static string URL
    {
        get
        {
            if (TestManager.TestServer)
                return TEST_URL;
            else
                return PRODUCT_URL;
        }
    }
    
    public const string ANDROID = "Android";
    public const string IOS = "IOS";

    public const string VersionForceUpdate = "Force_Update.txt";

#if UNITY_ANDROID
    public const string prefixKey = "Android";
#elif UNITY_IOS
    public const string prefixKey = "iOS";    
#endif

    [SerializeField] private int cacheLevelNumber;
    public const int timeOut = 4;
    public bool testLevel;
    public int maxLevel = 50;

    public string latest_version = "1.1.7";
    public List<string> lst_version_need_force_update;
    public bool fetchForceSucess;

    private Coroutine cacheCoroutine;

    public static string LevelURL
    {
        get
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WindowsEditor)
            {
                return URL + ANDROID;

            }
            else
            {
                return URL + IOS;

            }
        }
    }
    
    public static string ForceDataURL
    {
        get
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WindowsEditor)
                return PRODUCT_URL + ANDROID;
            else
                return PRODUCT_URL + IOS;
        }
    }

    private void Start()
    {
        Caching.ClearCache();        
        testLevel = true;
        fetchForceSucess = true;

        //FetchForceUpdateData();
        FetchLevels(UserDataManager.Level);
    }

    private void FetchForceUpdateData()
    {
        fetchForceSucess = false;
        StartCoroutine(FetchForceUpdateCoroutine());
    }

    private IEnumerator FetchForceUpdateCoroutine()
    {
        string url = ForceDataURL + "/" + VersionForceUpdate;
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.timeout = timeOut;
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success && !string.IsNullOrEmpty(request.downloadHandler.text))
        {
            ForceUpdateData data = JsonUtility.FromJson<ForceUpdateData>(request.downloadHandler.text);
            if (data != null && data.lst_version_name != null && !string.IsNullOrEmpty(data.latest_version))
            {
                Debug.Log($"[FetchForceUpdate] Fetch force update success: {request.downloadHandler.text}");
                ToastUtil.ShowToast($"Fetch force update success");

                latest_version = data.latest_version;
                lst_version_need_force_update = data.lst_version_name;
                fetchForceSucess = true;
                
                //Server.Get<OnFetchForceDataComplete>().Dispatch();
                //Manager.Instance.CheckForceUpdate();
                FetchLevels(UserDataManager.Level);
            }
            else
            {
                Debug.Log($"[FetchForceUpdate] Fetch force update failed: json data");
                ToastUtil.ShowToast($"Fetch force update failed by json data");
                yield return StartCoroutine(CheckCacheLevelId());
            }
        }
        else
        {
            Debug.Log($"[FetchForceUpdate] Fetch force update failed: {url}" );
            ToastUtil.ShowToast($"Fetch force update failed by {url}");
            yield return StartCoroutine(CheckCacheLevelId());
        }
    }

    private IEnumerator CheckCacheLevelId()
    {
        for (int i = 0; i < cacheLevelNumber; i++)
        {           
            int level = UserDataManager.Level + i;
            int levelId = level;
            // if (level > ResourcesManager.Instance.GetLevelNumber())
            // {
            //     levelId = ResourcesManager.Instance.GetLoopLevelId(level);
            // }
            if (UserDataManager.GetCacheLevel()[i] != null) 
            {
                ToastUtil.ShowToast($"Level {levelId} cache available");
                yield break;
            }

            TextAsset levelLocal = GetLevelLocal(levelId, UserDataManager.LevelSetID);
            yield return StartCoroutine(DecryptCoroutine(levelLocal.text, SaveLoadUtility.pass, (json) =>
            {
                ToastUtil.ShowToast($"Decrypt success: Level local {levelId}");
                UserDataManager.LevelData levelData = UserDataManager.LoadLevelData();
                levelData.cacheLevel[i] = SaveLoadUtility.CreateLevelConfigFromJson(json);
                UserDataManager.SaveLevelData(levelData);
            }));
        }
    }

    public void FetchLevels(int startLevel)
    {
        //if(!fetchForceSucess || (Manager.Instance.isForceUpdate && !Manager.Instance.hasUpdateVersionInStore)) return;
        StartCoroutine(FetchLevelsCoroutine(startLevel));
    }

    public IEnumerator FetchLevelsCoroutine(int startLevel)
    {
        for (int i = 0; i < cacheLevelNumber; i++)
        {           
            int level = startLevel + i;
            int levelId = level;
            if (level > maxLevel)
            {
                levelId = GetLoopLevelId(level);
            }
            Debug.Log($"[FetchLevel] start download level:{level} loop level id {levelId}");
            yield return StartCoroutine(DownloadAndDecryptLevel(levelId, i));
        }
    }    

    private IEnumerator DownloadAndDecryptLevel(int levelId, int cacheIndex)
    {
        string levelName = GetPerfectName(levelId);
        string url = LevelURL + "/" + levelName;

        ToastUtil.ShowToast($"Download:-{levelId} from {url}");        

        #region Download From Adressable

        //var handle = Addressables.LoadContentCatalogAsync($"{LevelURL}/catalog_catalog.json");
        //yield return handle;
        //Start load level        
        //var level = Addressables.LoadAssetAsync<LevelConfig>($"{prefixKey}/{levelName}");
        //yield return level;       

        //if(level.Result != null)
        //{
        //    Debug.Log($"[FetchLevel] download success:{levelNumber}");
        //    Debug.Log(level.Result.InputImageConfig.SavePath);
        //}
        //else
        //{
        //    downloadSucess = false;
        //    Debug.LogError("[FetchLevel] Fail Download level: " + levelNumber);
        //}
        #endregion

        #region Download From Server

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.timeout = timeOut;
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success && !string.IsNullOrEmpty(request.downloadHandler.text))
        {
            Debug.Log($"[FetchLevel] Download success:{levelId}");
            ToastUtil.ShowToast($"download success: Level {levelId}");

            yield return StartCoroutine(DecryptCoroutine(request.downloadHandler.text, SaveLoadUtility.pass, (json) =>
            {
                ToastUtil.ShowToast($"Decrypt success: Level server {levelId}");
                UserDataManager.LevelData levelData = UserDataManager.LoadLevelData();
                levelData.cacheLevel[cacheIndex] = SaveLoadUtility.CreateLevelConfigFromJson(json);
                UserDataManager.SaveLevelData(levelData);
            }));
        }
        else
        {
            Debug.LogError($"[FetchLevel] Download failed: {levelId}  URL: {url} error: {request.error}");                       
            ToastUtil.ShowToast($"Download fail: Level {levelId}");

            if (UserDataManager.GetCacheLevel()[cacheIndex] != null) 
            {
                if (UserDataManager.GetCacheLevel()[cacheIndex].levelId == levelId)
                {
                    ToastUtil.ShowToast($"Level {levelId} cache available");
                }
                else
                {
                    LevelConfig levelConfig = UserDataManager.CheckLevelExistInCache(levelId);
                    if (levelConfig != null)
                    {
                        UserDataManager.LevelData levelData = UserDataManager.LoadLevelData();
                        levelData.cacheLevel[cacheIndex] = levelConfig;
                        UserDataManager.SaveLevelData(levelData);
                        ToastUtil.ShowToast($"Level {levelId} cache available");
                    }
                    else
                    {
                        TextAsset levelLocal = GetLevelLocal(levelId, UserDataManager.LevelSetID);
                        yield return StartCoroutine(DecryptCoroutine(levelLocal.text, SaveLoadUtility.pass, (json) =>
                        {
                            ToastUtil.ShowToast($"Decrypt success: Level local {levelId}");
                            UserDataManager.LevelData levelData = UserDataManager.LoadLevelData();
                            levelData.cacheLevel[cacheIndex] = SaveLoadUtility.CreateLevelConfigFromJson(json);
                            UserDataManager.SaveLevelData(levelData);
                        }));
                    }
                }
            }
        }
        #endregion
    }

    private TextAsset GetLevelLocal(int levelId, int levelSetID)
    {
        var textLv = Resources.Load<TextAsset>($"Levels/{levelSetID}/Level_{levelId}");
        if (textLv == null)
        {
            textLv = Resources.Load<TextAsset>($"Levels/0/Level_{levelId}");
            if (textLv == null)
            {
                textLv = Resources.Load<TextAsset>($"Levels/0/Level_{1}");
            }
        }
        return textLv;
    }

    public static string GetPerfectName(int level)
    {
        return $"Level_{level}.bytes";
    }    

    public IEnumerator DecryptCoroutine(string base64Input, string password, System.Action<string> onComplete, int maxChunksPerFrame = 8)
    {
        byte[] data = Convert.FromBase64String(base64Input);
        byte[] iv = new byte[16];
        Array.Copy(data, 0, iv, 0, iv.Length);
        byte[] cipher = new byte[data.Length - iv.Length];
        Array.Copy(data, iv.Length, cipher, 0, cipher.Length);

        // 2. Derive key async
        Task<byte[]> deriveTask = Task.Run(() =>
        {
            using var kdf = new Rfc2898DeriveBytes(password, QT.FixedSalt, QT.KdfIterations, HashAlgorithmName.SHA256);
            return kdf.GetBytes(32);
        });
        while (!deriveTask.IsCompleted)
            yield return null;
        if (deriveTask.IsFaulted)
        {
            Debug.LogError($"KDF failed: {deriveTask.Exception}");
            yield break;
        }
        byte[] key = deriveTask.Result;

        // 3. Decrypt + decompress incremental
        using var aes = Aes.Create();
        aes.KeySize = 256;
        aes.BlockSize = 128;
        aes.Mode = CipherMode.CBC;
        aes.Key = key;
        aes.IV = iv;
        using var crypto = new CryptoStream(new MemoryStream(cipher), aes.CreateDecryptor(), CryptoStreamMode.Read);
        using var brotli = new BrotliStream(crypto, CompressionMode.Decompress);

        var msOut = new MemoryStream();
        byte[] buffer = new byte[4096];
        int bytesRead = 0;

        while (true)
        {
            for (int i = 0; i < maxChunksPerFrame; i++)
            {
                bytesRead = brotli.Read(buffer, 0, buffer.Length);
                if (bytesRead <= 0)
                    break;
                msOut.Write(buffer, 0, bytesRead);
            }
            if (bytesRead <= 0)
                break;
            yield return null;
        }

        string json = Encoding.UTF8.GetString(msOut.ToArray());
        onComplete?.Invoke(json);
    }

    [Button]
    public int GetLoopLevelId(int levelId)
    {
        return LevelLooper.GetLoopBaseLevelId(levelId, maxLevel, 9);
    }
}

[Serializable]
public class ForceUpdateData
{
    public string latest_version;
    public List<string> lst_version_name;
}