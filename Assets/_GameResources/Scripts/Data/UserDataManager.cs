using Yoolax.Framework;
using Newtonsoft.Json;
using System;
using Analytics;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class UserDataManager : MonoBehaviour
{
    public const int LevelMax = 200;
    [System.Serializable]
    public class UserData
    {
        public int level;
        public int levelSetID;
        public int gold;
        public int heart;
        public bool removeAds;
        public int addTrayBooster;
        public int shuffleBooster;
        public int magnetBooster;
        public int handBooster;
        public bool firstClaimAddTrayBooster;
        public bool firstClaimHandBooster;
        public bool firstClaimShuffleBooster;
        public bool firstClaimMagnetBooster;
        public long lastTimeLogin;
        public string userName;
        public long firstTimeJoinGame;
        public int avtarID = 1;
        public int isFirstShowChangeName;
        public int lastFeatureCount;
        public int buyIapCount;
        public int winStreak;
        public int loseStreak;
    }

    private const string USER_DATA_KEY = Constant.PlayerPrefs.USER_DATA;
    public static Action<int, int, float> OnUpdateGold;

    [System.Serializable]
    public class LevelData
    {
        public int playIndex;
        public int loseIndex;
        public int exitIndex;
        public PlayType playType;
    }
    
    private const string LEVEL_DATA_KEY = Constant.PlayerPrefs.LEVEL_DATA_KEY;

    #region Common

    public static void AddGold(int value, string where, string reason = "", float timeDelay = 0)
    {
        if (where != " ")
        {
            string[] types = { ResourceType.currency.ToString() };
            string[] names = { "coin" };
            string[] amounts = { value.ToString() };            
            ResourceAnalyticStruct resourceAnalyticStruct = new ResourceAnalyticStruct(types, names, amounts, 
                where, reason);
            if(value >= 0)
                Server.Get<OnResourceEarnEventLog>().Dispatch(resourceAnalyticStruct);
            else
            {
                Server.Get<OnResourceSpendEventLog>().Dispatch(resourceAnalyticStruct);
            }
        }
        Gold = Mathf.Clamp(Gold + value, 0, int.MaxValue);
        OnUpdateGold?.Invoke(Gold + value, Gold, timeDelay);
    }

    public static Action<int, int, bool> OnAddHeart;
    public static Action AvatarChange;
    public static Action UserNameChange;

    public static bool IsNewDay = false;

    public static void AddHeart(int amount, string where, bool hasAnimation, int typeHeart = 0, string reason = "")
    {
        if (typeHeart == 0)
        {
            if (where != " ")
            {
                string[] types = { ResourceType.currency.ToString() };
                string[] names = { "heart" };
                string[] amounts = { amount.ToString() };            
                ResourceAnalyticStruct resourceAnalyticStruct = new ResourceAnalyticStruct(types, names, amounts, 
                    where, reason);
                if(amount >= 0)
                    Server.Get<OnResourceEarnEventLog>().Dispatch(resourceAnalyticStruct);
                else
                {
                    Server.Get<OnResourceSpendEventLog>().Dispatch(resourceAnalyticStruct);
                }
            }
            int current = Heart;
            int newValue = current + amount;
            if (newValue < 0)
            {
                newValue = 0;
            }

            if (newValue > 5)
            {
                newValue = 5;
            }
            Heart = newValue;
            OnAddHeart?.Invoke(current, newValue, hasAnimation);
        }
        else
        {
            if (where != " ")
            {
                string[] types = { ResourceType.currency.ToString() };
                string[] names = { "infinity_lives" };
                string[] amounts = { amount.ToString() };            
                ResourceAnalyticStruct resourceAnalyticStruct = new ResourceAnalyticStruct(types, names, amounts, 
                    where, reason);
                if(amount >= 0)
                    Server.Get<OnResourceEarnEventLog>().Dispatch(resourceAnalyticStruct);
                else
                {
                    Server.Get<OnResourceSpendEventLog>().Dispatch(resourceAnalyticStruct);
                }
            }
            var now = GameTime.Instance.GetUtcTime();
            if (HeartManager.InfinityEndTime < now)
            {
                HeartManager.InfinityEndTime = now + amount;
            }
            else
            {
                HeartManager.InfinityEndTime += amount;
            }
            OnAddHeart?.Invoke(Heart, Heart, hasAnimation);
        }
    }
    #endregion

    #region Get Set
    public static int LastFeatureCount
    {
        get { return LoadUserData().lastFeatureCount; }
        set
        {
            UserData data = LoadUserData();
            data.lastFeatureCount = value;
            SaveUserData(data);
        }
    }
    public static int Level
    {
        get { return LoadUserData().level; }
        set
        {
            UserData data = LoadUserData();
            data.level = value;
            SaveUserData(data);
        }
    }
    public static int avtarID
    {
        get
        {
            return LoadUserData().avtarID;
        }
        set
        {
            UserData data = LoadUserData();
            data.avtarID = value;
            SaveUserData(data);
            AvatarChange?.Invoke();
        }
    }

    public static string userName
    {
        get
        {
            string nameCurrent = LoadUserData().userName;
            if (string.IsNullOrEmpty(nameCurrent))
            {
                UserData data = LoadUserData();
                data.userName = getNewUserName();
                nameCurrent = data.userName;
                SaveUserData(data);
            }
            return nameCurrent;
        }
        set
        {
            UserData data = LoadUserData();
            data.userName = value;
            SaveUserData(data);
            UserNameChange?.Invoke();
        }
    }
    private static string getNewUserName()
    {
        return $"User#{UnityEngine.Random.Range(0, 10000000):0000}";
    }

    public static long firstTimeJoinGame
    {
        get { return LoadUserData().firstTimeJoinGame; }
        set
        {
            UserData data = LoadUserData();
            data.firstTimeJoinGame = value;
            SaveUserData(data);
        }
    }

    public static long LastTimeLogin
    {
        get { return LoadUserData().lastTimeLogin; }
        set
        {
            UserData data = LoadUserData();
            data.lastTimeLogin = value;
            SaveUserData(data);
        }
    }

    public static int Gold
    {
        get
        {
            UserData data = LoadUserData();
            return data.gold;
        }
        private set
        {
            UserData data = LoadUserData();
            data.gold = value;
            SaveUserData(data);
        }
    }

    public static int Heart
    {
        get
        {
            UserData data = LoadUserData();
            return data.heart;
        }
        private set
        {
            UserData data = LoadUserData();
            data.heart = value;
            SaveUserData(data);
        }
    }

    public static bool RemoveAds
    {
        get { return LoadUserData().removeAds; }
        set
        {
            UserData data = LoadUserData();
            data.removeAds = value;
            SaveUserData(data);
        }
    }

    public static int LevelSetID
    {
        get { return LoadUserData().levelSetID; }
        set
        {
            UserData data = LoadUserData();
            data.levelSetID = value;
            SaveUserData(data);
        }
    }
    public static bool IsFirstShowChangeName
    {
        get
        {
            return LoadUserData().isFirstShowChangeName == 0;
        }
        set
        {
            UserData data = LoadUserData();
            data.isFirstShowChangeName = value ? 0 : 1;
            SaveUserData(data);
        }
    }

    public static int AddTrayBooster
    {
        get { return LoadUserData().addTrayBooster; }
        set
        {
            UserData data = LoadUserData();
            data.addTrayBooster = value;
            SaveUserData(data);
        }
    }

    public static int ShuffleBooster
    {
        get { return LoadUserData().shuffleBooster; }
        set
        {
            UserData data = LoadUserData();
            data.shuffleBooster = value;
            SaveUserData(data);
        }
    }

    public static int MagnetBooster
    {
        get { return LoadUserData().magnetBooster; }
        set
        {
            UserData data = LoadUserData();
            data.magnetBooster = value;
            SaveUserData(data);
        }
    }

    public static int HandBooster
    {
        get { return LoadUserData().handBooster; }
        set
        {
            UserData data = LoadUserData();
            data.handBooster = value;
            SaveUserData(data);
        }
    }
    
    public static bool FirstClaimAddTrayBooster
    {
        get { return LoadUserData().firstClaimAddTrayBooster; }
        set
        {
            UserData data = LoadUserData();
            data.firstClaimAddTrayBooster = value;
            SaveUserData(data);
        }
    }
    
    public static bool FirstClaimHandBooster
    {
        get { return LoadUserData().firstClaimHandBooster; }
        set
        {
            UserData data = LoadUserData();
            data.firstClaimHandBooster = value;
            SaveUserData(data);
        }
    }
    
    public static bool FirstClaimShuffleBooster
    {
        get { return LoadUserData().firstClaimShuffleBooster; }
        set
        {
            UserData data = LoadUserData();
            data.firstClaimShuffleBooster = value;
            SaveUserData(data);
        }
    }
    
    public static bool FirstClaimSuperShoot
    {
        get { return LoadUserData().firstClaimMagnetBooster; }
        set
        {
            UserData data = LoadUserData();
            data.firstClaimMagnetBooster = value;
            SaveUserData(data);
        }
    }
    
    public static int BuyIapCount
    {
        get { return LoadUserData().buyIapCount; }
        set
        {
            UserData data = LoadUserData();
            data.buyIapCount = value;
            SaveUserData(data);
        }
    }
    
    public static int WinStreak
    {
        get { return LoadUserData().winStreak; }
        set
        {
            UserData data = LoadUserData();
            data.winStreak = value;
            SaveUserData(data);
        }
    }

    public static int LoseStreak
    {
        get { return LoadUserData().loseStreak; }
        set
        {
            UserData data = LoadUserData();
            data.loseStreak = value;
            SaveUserData(data);
        }
    }
    
    public static int PlayIndex
    {
        get { return LoadLevelData().playIndex; }
        set
        {
            LevelData data = LoadLevelData();
            data.playIndex = value;
            SaveLevelData(data);
        }
    }
    
    public static int LoseIndex
    {
        get { return LoadLevelData().loseIndex; }
        set
        {
            LevelData data = LoadLevelData();
            data.loseIndex = value;
            SaveLevelData(data);
        }
    }
    
    public static int ExitIndex
    {
        get { return LoadLevelData().exitIndex; }
        set
        {
            LevelData data = LoadLevelData();
            data.exitIndex = value;
            SaveLevelData(data);
        }
    }
    
    public static PlayType PlayType
    {
        get { return LoadLevelData().playType; }
        set
        {
            LevelData data = LoadLevelData();
            data.playType = value;
            SaveLevelData(data);
        }
    }

    public static void AddBooster(BoosterType boosterType, int quantity)
    {
        switch(boosterType)
        {
            case BoosterType.ADD_TRAY:
                AddTrayBooster += quantity;
                break;
            case BoosterType.SHUFFLE:
                ShuffleBooster += quantity;
                break;
            case BoosterType.HAND_MOVE:
                HandBooster += quantity;
                break;
            case BoosterType.SUPER_SHOOT:
                MagnetBooster += quantity;
                break;
            default:
                break;
        }
    }
        

    public static UserData LoadUserData()
    {
        if (PlayerPrefs.HasKey(USER_DATA_KEY))
        {
            string jsonData = PlayerPrefs.GetString(USER_DATA_KEY);
            return JsonConvert.DeserializeObject<UserData>(jsonData);
        }
        return GetDefaultUserData();
    }

    private static void SaveUserData(UserData data)
    {
        string jsonData = JsonConvert.SerializeObject(data);
        PlayerPrefs.SetString(USER_DATA_KEY, jsonData);
        PlayerPrefs.Save();
    }
    
    public static LevelData LoadLevelData()
    {
        if (PlayerPrefs.HasKey(LEVEL_DATA_KEY))
        {
            string jsonData = PlayerPrefs.GetString(LEVEL_DATA_KEY);
            return JsonConvert.DeserializeObject<LevelData>(jsonData);
        }
        return GetDefaultLevelData();
    }

    private static void SaveLevelData(LevelData data)
    {
        string jsonData = JsonConvert.SerializeObject(data);
        PlayerPrefs.SetString(LEVEL_DATA_KEY, jsonData);
        PlayerPrefs.Save();
    }
    #endregion


    private static UserData GetDefaultUserData()
    {
        return new UserData
        {
            level = 1,
            gold = 0,
            heart = 5,
            removeAds = false,
            addTrayBooster = 0,
            shuffleBooster = 0,
            magnetBooster = 0,
            handBooster = 0,
            firstClaimAddTrayBooster = false,
            firstClaimHandBooster = false,
            firstClaimShuffleBooster = false,
            firstClaimMagnetBooster = false,
            buyIapCount = 0,
            winStreak = 0,
            loseStreak = 0,
        };
    }
    
    private static LevelData GetDefaultLevelData()
    {
        return new LevelData
        {
            playIndex = 0,
            loseIndex = 0,
            exitIndex = 0,
            playType = PlayType.home
        };
    }
}