using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush
{
    [Serializable]
    public class RewardDataGameGecko : RewardData
    {
        public override void Claim()
        {
            switch (type)
            {
                case ItemType.GOLD:
                    UserDataManager.AddGold(quantity, "Shop");
                    break;
                case ItemType.INFINITY_LIVES:
                    UserDataManager.AddHeart(quantity * 1000, "Reward", true, typeHeart:1);
                    break;
                case ItemType.BOOSTER_1:
                    UserDataManager.AddTrayBooster += quantity;
                    break;
                case ItemType.BOOSTER_2:
                    UserDataManager.HandBooster += quantity;
                    break;
                case ItemType.BOOSTER_3:
                    UserDataManager.ShuffleBooster += quantity;
                    break;
                case ItemType.BOOSTER_4:
                    UserDataManager.MagnetBooster += quantity;
                    break;
            }
        }
    }
}
