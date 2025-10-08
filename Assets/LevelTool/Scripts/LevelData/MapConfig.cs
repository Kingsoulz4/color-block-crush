using System;
using System.Collections.Generic;
using UnityEngine;

namespace Geckout.Tools
{
    [Serializable]
    public class MapConfig
    {
        public Vector2Int mapSize;
        public List<CellConfig> cells = new List<CellConfig>();
    }
}
