using Yoolax.Framework;
using Firebase.Analytics;
using UnityEngine;
using System.Collections.Generic;

namespace Analytics
{
    public static class LevelAnalytics
    {
        public static void InitEvent()
        {
            Server.Get<OnLevelStartEventLog>().AddListener(LogLevelStartEvent);
            Server.Get<OnLevelCompleteEventLog>().AddListener(LogLevelCompleteEvent);
            Server.Get<OnLevelFailEventLog>().AddListener(LogLevelFailEvent);
        }

        public static void LogLevelStartEvent(LevelAnalyticStruct levelStruct)
        {
            var parameters = new[]
            {
                new Parameter("isLoop", levelStruct.isLoop),
                new Parameter("isFirst", levelStruct.isFirst),
                new Parameter("bc_AddTray", levelStruct.bc_AddTray),
                new Parameter("bc_Hand", levelStruct.bc_Hand),
                new Parameter("bc_Shuffle", levelStruct.bc_Shuffle),
                new Parameter("bc_Magnet", levelStruct.bc_Magnet),
                new Parameter("coinBalance", levelStruct.coinBalance),
                new Parameter("lifeBalance", levelStruct.lifeBalance),
            };
            Dictionary<string, object> paramData = new Dictionary<string, object>
            {
                { "isLoop", levelStruct.isLoop },
                { "isFirst", levelStruct.isFirst },
                { "bc_AddTray", levelStruct.bc_AddTray },
                { "bc_Hand", levelStruct.bc_Hand },
                { "bc_Shuffle", levelStruct.bc_Shuffle },
                { "bc_Magnet", levelStruct.bc_Magnet },
                { "coinBalance", levelStruct.coinBalance },
                {"lifeBalance", levelStruct.lifeBalance}
            };
            FirebaseManager.Instance.AddEvent("LevelStart", paramData, parameters);
        }

        public static void LogLevelCompleteEvent(LevelAnalyticStruct levelStruct)
        {
            var parameters = new[]
            {
                new Parameter("isLoop", levelStruct.isLoop),
                new Parameter("but_AddTray", levelStruct.but_AddTray),
                new Parameter("but_Hand", levelStruct.but_Hand),
                new Parameter("but_Shuffle", levelStruct.but_Shuffle),
                new Parameter("but_Magnet", levelStruct.but_Magnet),
                new Parameter("bu_AddTray", levelStruct.bu_AddTray),
                new Parameter("bu_Hand", levelStruct.bu_Hand),
                new Parameter("bu_Shuffle", levelStruct.bu_Shuffle),
                new Parameter("bu_Magnet", levelStruct.bu_Magnet),
                new Parameter("reviveAdsTotal", levelStruct.reviveAdsTotal),
                new Parameter("reviveCoinTotal", levelStruct.reviveCoinTotal),
                new Parameter("reviveAds", levelStruct.reviveAds),
                new Parameter("reviveCoin", levelStruct.reviveCoin),
                new Parameter("failQty", levelStruct.failQty),
                new Parameter("timePlay", levelStruct.timePlay),
                new Parameter("totalTimePlay", levelStruct.totalTimePlay),
                new Parameter("levelAttempt", levelStruct.levelAttempt),
                new Parameter("coinReward", levelStruct.coinReward)
            };

            Dictionary<string, object> paramData = new Dictionary<string, object>
            {
                { "isLoop", levelStruct.isLoop },
                { "but_AddTray", levelStruct.but_AddTray },
                { "but_Hand", levelStruct.but_Hand },
                { "but_Shuffle", levelStruct.but_Shuffle },
                { "but_Magnet", levelStruct.but_Magnet },
                { "bu_AddTray", levelStruct.bu_AddTray },
                { "bu_Hand", levelStruct.bu_Hand },
                { "bu_Shuffle", levelStruct.bu_Shuffle },
                { "bu_Magnet", levelStruct.bu_Magnet },
                { "reviveAdsTotal", levelStruct.reviveAdsTotal },
                { "reviveCoinTotal", levelStruct.reviveCoinTotal },
                { "reviveAds", levelStruct.reviveAds },
                { "reviveCoin", levelStruct.reviveCoin },
                { "failQty", levelStruct.failQty },
                { "timePlay", levelStruct.timePlay },
                { "totalTimePlay", levelStruct.totalTimePlay },
                { "levelAttempt", levelStruct.levelAttempt },
                { "coinReward", levelStruct.coinReward }
            };
            FirebaseManager.Instance.AddEvent("LevelComplete", paramData, parameters);
        }

        public static void LogLevelFailEvent(LevelAnalyticStruct levelStruct)
        {
            var parameters = new[]
            {
                new Parameter("failReason", levelStruct.failReason),
                new Parameter("isLoop", levelStruct.isLoop),
                new Parameter("bu_AddTray", levelStruct.bu_AddTray),
                new Parameter("bu_Hand", levelStruct.bu_Hand),
                new Parameter("bu_Shuffle", levelStruct.bu_Shuffle),
                new Parameter("bu_Magnet", levelStruct.bu_Magnet),
                new Parameter("reviveAds", levelStruct.reviveAds),
                new Parameter("reviveCoin", levelStruct.reviveCoin),
                new Parameter("timePlay", levelStruct.timePlay),
                new Parameter("objectTotal", levelStruct.objectTotal),
                new Parameter("objectUnslove", levelStruct.objectUnslove),
            };
            Dictionary<string, object> paramData = new Dictionary<string, object>
            {
                { "failReason", levelStruct.failReason },
                { "isLoop", levelStruct.isLoop },
                { "bu_AddTray", levelStruct.bu_AddTray },
                { "bu_Shuffle", levelStruct.bu_Shuffle },
                { "bu_Hand", levelStruct.bu_Hand },
                { "bu_Magnet", levelStruct.bu_Magnet },
                { "reviveAds", levelStruct.reviveAds },
                { "reviveCoin", levelStruct.reviveCoin },
                { "timePlay", levelStruct.timePlay },
                { "objectTotal", levelStruct.objectTotal },
                { "objectUnslove", levelStruct.objectUnslove },
            };
            FirebaseManager.Instance.AddEvent("LevelFail", paramData, parameters);
        }
    }

    public struct LevelAnalyticStruct
    {
        public int isLoop;
        public int isFirst;
        public int bc_AddTray;
        public int bc_Hand;
        public int bc_Shuffle;
        public int bc_Magnet;
        public int coinBalance;
        public int lifeBalance;
        public int bu_AddTray;
        public int bu_Hand;
        public int bu_Shuffle;
        public int bu_Magnet;
        public int but_AddTray;
        public int but_Hand;
        public int but_Shuffle;
        public int but_Magnet;
        public int reviveAdsTotal;
        public int reviveCoinTotal;
        public int reviveAds;
        public int reviveCoin;
        public int failQty;
        public int timePlay;
        public int totalTimePlay;
        public int levelAttempt;
        public int coinReward;
        public string failReason;
        public int objectTotal;
        public int objectUnslove;

        public LevelAnalyticStruct SetBaseLevel(int isLoop)
        {
            this.isLoop = isLoop;
            return this;
        }

        public LevelAnalyticStruct SetLevelStartStruct(int isFirst, int bc_AddTray, int bc_Hand, int bc_Shuffle,
            int bc_Magnet, int coinBalance, int lifeBalance)
        {
            this.isFirst = isFirst;
            this.bc_AddTray = bc_AddTray;
            this.bc_Hand = bc_Hand;
            this.bc_Shuffle = bc_Shuffle;
            this.bc_Magnet = bc_Magnet;
            this.coinBalance = coinBalance;
            this.lifeBalance = lifeBalance;
            return this;
        }

        public LevelAnalyticStruct SetLevelCompleteStruct(int but_AddTray, int but_Hand, int but_Shuffle,
            int but_Magnet,
            int bu_AddTray, int bu_Hand, int bu_Shuffle, int bu_Magnet, int reviveAdsTotal, int reviveCoinTotal,
            int reviveAds, int reviveCoin, int failQty, int timeplay, int totalTimePlay, int levelAttempt,
            int coinReward)
        {
            this.but_AddTray = but_AddTray;
            this.but_Hand = but_Hand;
            this.but_Shuffle = but_Shuffle;
            this.but_Magnet = but_Magnet;
            this.bu_AddTray = bu_AddTray;
            this.bu_Hand = bu_Hand;
            this.bu_Shuffle = bu_Shuffle;
            this.bu_Magnet = bu_Magnet;
            this.reviveAdsTotal = reviveAdsTotal;
            this.reviveCoinTotal = reviveCoinTotal;
            this.reviveAds = reviveAds;
            this.reviveCoin = reviveCoin;
            this.failQty = failQty;
            this.timePlay = timeplay;
            this.totalTimePlay = totalTimePlay;
            this.levelAttempt = levelAttempt;
            this.coinReward = coinReward;
            return this;
        }

        public LevelAnalyticStruct SetLevelFailStruct(string failReason, int bu_AddTray, int bu_Hand, int bu_Shuffle,
            int bu_Magnet, int reviveAds, int reviveCoin, int objectTotal, int objectUnsolve)
        {
            this.failReason = failReason;
            this.bu_AddTray = bu_AddTray;
            this.bu_Hand = bu_Hand;
            this.bu_Shuffle = bu_Shuffle;
            this.bu_Magnet = bu_Magnet;
            this.reviveAds = reviveAds;
            this.reviveCoin = reviveCoin;
            this.objectTotal = objectTotal;
            this.objectUnslove = objectUnsolve;
            return this;
        }
    }

    public enum PlayType
    {
        home,
        next,
        restart
    }

    public enum LevelResult
    {
        win,
        lose,
        quit,
        restart
    }

    public enum FailReason
    {
        Replay,
        BackToHome,
        Normal,
        ResetProcess
    }
}