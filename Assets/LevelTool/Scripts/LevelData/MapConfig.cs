using System;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush.Tools
{
    [Serializable]
    public class MapConfig
    {
        public Vector2Int mapSize;
        public List<BlockConfig> blocks = new List<BlockConfig>();
        public List<BigBlockConfig> bigBlocks = new List<BigBlockConfig>();
        public List<KeyConfig> keys = new List<KeyConfig>();
        

        public MapConfig()
        {
            mapSize = Vector2Int.zero;
            blocks = new List<BlockConfig>();
            bigBlocks = new List<BigBlockConfig>();
            keys = new List<KeyConfig>();
        }
    }
}
