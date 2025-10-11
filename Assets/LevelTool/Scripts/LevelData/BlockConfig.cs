using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush.Tools
{
    [Serializable]
    public class BlockConfig
    {
        public int blockGroupId;
        public ColorType colorType;
        public int blockHealth;
        public List<int> cellsId;

        public BlockConfig()
        {
            cellsId = new List<int>();
        }
    }
}
