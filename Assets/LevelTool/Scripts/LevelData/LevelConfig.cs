using System;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush.Tools
{
    [CreateAssetMenu(fileName = "NewLevelConfig", menuName = "GameConfigs/LevelConfig")]
    public class LevelConfig : ScriptableObject
    {
        public int levelId;
        public LevelDifficult levelDifficult;

        public InputImageConfig imageConfig;
        public MapConfig mapConfig;
        public List<TankLineConfig> tankLines = new List<TankLineConfig>();

        public bool ValidateData()
        {
            return true;
        }
    }
}
