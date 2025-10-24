using System;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush.Tools
{
    [Serializable]
    public class LevelConfig
    {
        public int levelId;
        public LevelDifficult levelDifficult;

        public InputImageConfig imageConfig;
        public MapConfig mapConfig;
        public List<GunLineConfig> gunLines = new List<GunLineConfig>();

        public bool ValidateData()
        {
            return true;
        }
    }
}
