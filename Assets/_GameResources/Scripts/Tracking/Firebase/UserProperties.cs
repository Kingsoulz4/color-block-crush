using Yoolax.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace Analytics
{
    public static class UserProperties
    {
        public static void InitEvent()
        {
            Server.Get<OnUserPropertiesLog>().AddListener(LogUserPropertiesFireBase);
        }

        public static void LogUserPropertiesFireBase(UserPropertiesStruct userProperties)
        {
            FirebaseManager.Instance.AddQueueProperty("current_level", userProperties.currentLevel.ToString());
            FirebaseManager.Instance.AddQueueProperty("current_mode", userProperties.mode.ToString());
            FirebaseManager.Instance.AddQueueProperty("connection_type", userProperties.connectType.ToString());
            FirebaseManager.Instance.AddQueueProperty("firebase_exp", userProperties.firebaseExp);
            FirebaseManager.Instance.AddQueueProperty("is_iap_user_n", userProperties.isIapUser.ToString());
            FirebaseManager.Instance.AddQueueProperty("iap_count_n", userProperties.iapCount.ToString());
            FirebaseManager.Instance.AddQueueProperty("win_streak_n", userProperties.winStreak.ToString());
            FirebaseManager.Instance.AddQueueProperty("lose_streak_n", userProperties.loseStreak.ToString());
            FirebaseManager.Instance.AddQueueProperty("balance_coin_n", userProperties.balanceCoin.ToString());
            FirebaseManager.Instance.AddQueueProperty("balance_heart_n", userProperties.balanceHeart.ToString());
            FirebaseManager.Instance.AddQueueProperty("balance_shuffle_n", userProperties.balanceShuffle.ToString());
            FirebaseManager.Instance.AddQueueProperty("balance_undo_n", userProperties.balanceUndo.ToString());
            FirebaseManager.Instance.AddQueueProperty("balance_magnet_n", userProperties.balanceMagnet.ToString());
            FirebaseManager.Instance.AddQueueProperty("balance_add_space_n", userProperties.balanceAddSpace.ToString());
            FirebaseManager.Instance.AddQueueProperty("balance_revive_n", userProperties.balanceRevive.ToString());
            FirebaseManager.Instance.AddQueueProperty("country_ranking_n", userProperties.countryRanking.ToString());
            FirebaseManager.Instance.AddQueueProperty("global_ranking_n", userProperties.globalRanking.ToString());
            FirebaseManager.Instance.AddQueueProperty("current_event", userProperties.currentEvent.ToString());
            FirebaseManager.Instance.AddQueueProperty("current_event_index_n", userProperties.currentEventIndex.ToString());
            FirebaseManager.Instance.AddQueueProperty("current_event_cycle_n", userProperties.currentEventCycle.ToString());
        }

        #region Send Databucket       

        public static void AddPropertyToEvent(Dictionary<string, object> propertyData)
        {
            UserPropertiesStruct userProperties = AnalyticUtils.GetCurrentUserProperty();            
            
            propertyData["current_level"] = userProperties.currentLevel;
            propertyData["current_mode"] = userProperties.mode.ToString();
            propertyData["connection_type"] = userProperties.connectType.ToString();
            propertyData["firebase_exp"] = userProperties.firebaseExp;
            propertyData["is_iap_user_n"] = userProperties.isIapUser;
            propertyData["iap_count_n"] = userProperties.iapCount;
            propertyData["win_streak_n"] = userProperties.winStreak;
            propertyData["lose_streak_n"] = userProperties.loseStreak;
            propertyData["balance_coin_n"] = userProperties.balanceCoin;
            propertyData["balance_heart_n"] = userProperties.balanceHeart;
            propertyData["balance_shuffle_n"] = userProperties.balanceShuffle;
            propertyData["balance_undo_n"] = userProperties.balanceUndo;
            propertyData["balance_magnet_n"] = userProperties.balanceMagnet;
            propertyData["balance_add_space_n"] = userProperties.balanceAddSpace;
            propertyData["balance_revive_n"] = userProperties.balanceRevive;
            propertyData["country_ranking_n"] = userProperties.countryRanking;
            propertyData["global_ranking_n"] = userProperties.globalRanking;
            propertyData["current_event"] = userProperties.currentEvent;
            propertyData["current_event_index_n"] = userProperties.currentEventIndex;
            propertyData["current_event_cycle_n"] = userProperties.currentEventCycle;
        }

        #endregion
    }

    public struct UserPropertiesStruct
    {
        public int currentLevel;
        public Mode mode;
        public ConnectType connectType;
        public string firebaseExp;
        public int isIapUser;
        public int iapCount;
        public int winStreak;
        public int loseStreak;
        public int balanceCoin;
        public int balanceHeart;
        public int balanceShuffle;
        public int balanceUndo;
        public int balanceMagnet;
        public int balanceAddSpace;
        public int balanceRevive;
        public int countryRanking;
        public int globalRanking;
        public string currentEvent;
        public string currentEventIndex;
        public string currentEventCycle;
    };

    public enum Mode
    {
        normal,
        challenge,
        endless
    }

    public enum ConnectType
    {
        offline,
        online,
        wifi,
        mobile_data,
        unknown
    }
}
