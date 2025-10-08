using System;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush.Tools
{
    [Serializable]
    public class LevelConfig : MonoBehaviour
    {
        public int levelId;
        public LevelDifficult levelDifficult;

        public InputImageConfig imageConfig;
        public MapConfig mapConfig;
        public List<TankLineConfig> tankLines = new List<TankLineConfig>();
    }
}
