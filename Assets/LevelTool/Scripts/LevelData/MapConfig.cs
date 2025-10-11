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

        public MapConfig()
        {
            mapSize = Vector2Int.zero;
            cells = new List<CellConfig>();
        }
    }
}
