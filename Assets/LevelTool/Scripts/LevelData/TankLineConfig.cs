using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush.Tools
{
    [Serializable]
    public class TankLineConfig
    {
        public List<TankLineElementConfig> tankLineElementConfigs = new List<TankLineElementConfig>();

        public TankLineElementConfig GetTankConfig(int index)
        {
            if (index < 0 || index >= tankLineElementConfigs.Count)
            {
                Debug.LogError($"index out of range: {index}");
                return null;
            }
            
            return tankLineElementConfigs[index];
        }
    }
}
