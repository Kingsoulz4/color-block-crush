using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush.Tools
{
    [Serializable]
    public class GunLineConfig
    {
        public List<GunLineElementConfig> gunLineElementConfigs = new List<GunLineElementConfig>();

        public GunLineElementConfig GetGunLineElementConfig(int index)
        {
            if (index < 0 || index >= gunLineElementConfigs.Count)
            {
                Debug.LogError($"index out of range: {index}");
                return null;
            }
            
            return gunLineElementConfigs[index];
        }
    }
}
