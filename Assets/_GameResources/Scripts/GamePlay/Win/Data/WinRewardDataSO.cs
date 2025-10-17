using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush
{
    [CreateAssetMenu(fileName = "WinRewardDataSO", menuName = "ScriptableObjects/WinRewardDataSO", order = 1)]
    public class WinRewardDataSO : ScriptableObject
    {
        public int normalCoinReward;
        public int hardCoinReward;
        public int superHardCoinReward;
    }
}
