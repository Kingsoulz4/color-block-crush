using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush.Tools
{
    [Serializable]
    public class BigBlockConfig
    {
        public int bigBlockId;
        public ColorType colorType;
        public int blockHealth;
        public List<int> blocksId;

        public BigBlockConfig()
        {
            blocksId = new List<int>();
        }
    }
}
