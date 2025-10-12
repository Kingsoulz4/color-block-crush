using System;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush.Tools
{
    [Serializable]
    public class MapConfig
    {
        public Vector2Int mapSize;
        public List<CellConfig> cells = new List<CellConfig>();
        public List<BlockConfig> blocks = new List<BlockConfig>();
        public List<KeyConfig> keys = new List<KeyConfig>();
        

        public MapConfig()
        {
            mapSize = Vector2Int.zero;
            cells = new List<CellConfig>();
            blocks = new List<BlockConfig>();
            keys = new List<KeyConfig>();
        }
    }
}
