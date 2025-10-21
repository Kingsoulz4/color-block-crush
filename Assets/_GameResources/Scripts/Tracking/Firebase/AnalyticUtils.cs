using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Analytics
{
    public static class AnalyticUtils
    {
        #region User Property

        public static UserPropertiesStruct GetCurrentUserProperty()
        {
            UserPropertiesStruct userPropertyStruct = new UserPropertiesStruct();

            return userPropertyStruct;
        }

        public static Analytics.ConnectType GetConnectionTypeFromUnityApi()
        {
            var reach = Application.internetReachability;
            switch (reach)
            {
                case NetworkReachability.NotReachable:
                    return ConnectType.offline;
                case NetworkReachability.ReachableViaLocalAreaNetwork:
                    return ConnectType.wifi;
                case NetworkReachability.ReachableViaCarrierDataNetwork:
                    return ConnectType.mobile_data;
                default:
                    return ConnectType.unknown;
            }
        }

        #endregion

        #region Level

        private const int maxLevel = 250;

        public static LevelAnalyticStruct SetBaseLevel(this LevelAnalyticStruct levelBaseStruct)
        {
            levelBaseStruct.SetBaseLevel(0);
            return levelBaseStruct;
        }        

        #endregion
    }
}

