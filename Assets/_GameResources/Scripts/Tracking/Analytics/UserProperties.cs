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
            FirebaseManager.Instance.AddQueueProperty("balance_add_tray_n", userProperties.balanceAddTray.ToString());
            FirebaseManager.Instance.AddQueueProperty("balance_hand_n", userProperties.balanceHand.ToString());
            FirebaseManager.Instance.AddQueueProperty("balance_shuffle_n", userProperties.balanceShuffle.ToString());
            FirebaseManager.Instance.AddQueueProperty("balance_super_shoot_n", userProperties.balanceSuperShoot.ToString());
            FirebaseManager.Instance.AddQueueProperty("balance_revive_n", userProperties.balanceRevive.ToString());
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
            propertyData["balance_add_tray_n"] = userProperties.balanceAddTray;
            propertyData["balance_hand_n"] = userProperties.balanceHand;
            propertyData["balance_shuffle_n"] = userProperties.balanceShuffle;
            propertyData["balance_super_shoot_n"] = userProperties.balanceSuperShoot;
            propertyData["balance_revive_n"] = userProperties.balanceRevive;
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
        public int balanceAddTray;
        public int balanceHand;
        public int balanceShuffle;
        public int balanceSuperShoot;
        public int balanceRevive;
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
