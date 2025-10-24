using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
                    UserDataManager.AddGold(quantity, " ");
                    break;
                case ItemType.INFINITY_LIVES:
                    UserDataManager.AddHeart(quantity * 1000, " ", true, typeHeart:1);
                    break;
                case ItemType.BOOSTER_1:
                    UserDataManager.AddTrayBooster += quantity;
                    if (BoosterManager.Instance != null)
                    {
                        BoosterBase addTrayBooster = BoosterManager.Instance.Boosters.FirstOrDefault(x =>
                            x.BoosterType == BoosterType.ADD_TRAY);
                        if (addTrayBooster != null)
                        {
                            addTrayBooster.UpdateVisualBooster();
                        }
                    }
                    break;
                case ItemType.BOOSTER_2:
                    UserDataManager.HandBooster += quantity;
                    if (BoosterManager.Instance != null)
                    {
                        BoosterBase handBooster = BoosterManager.Instance.Boosters.FirstOrDefault(x =>
                            x.BoosterType == BoosterType.HAND_MOVE);
                        if (handBooster != null)
                        {
                            handBooster.UpdateVisualBooster();
                        }
                    }
                    break;
                case ItemType.BOOSTER_3:
                    UserDataManager.ShuffleBooster += quantity;
                    if (BoosterManager.Instance != null)
                    {
                        BoosterBase shuffleBooster = BoosterManager.Instance.Boosters.FirstOrDefault(x =>
                            x.BoosterType == BoosterType.SHUFFLE);
                        if (shuffleBooster != null)
                        {
                            shuffleBooster.UpdateVisualBooster();
                        }
                    }
                    break;
                case ItemType.BOOSTER_4:
                    UserDataManager.MagnetBooster += quantity;
                    if (BoosterManager.Instance != null)
                    {
                        BoosterBase superShootBooster = BoosterManager.Instance.Boosters.FirstOrDefault(x =>
                            x.BoosterType == BoosterType.SUPER_SHOOT);
                        if (superShootBooster != null)
                        {
                            superShootBooster.UpdateVisualBooster();
                        }
                    }
                    break;
            }
        }
    }
}
