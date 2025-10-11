using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush.Tools
{
    [Serializable]
    public class CellConfig
    {
        [Header("Base Infor")]
        public int id;
        public Vector2Int coordinate;
        public ColorType colorType;

        [Header("Key")]
        public int keyId;

        [Header("Block")] 
        public int blockGroupId;

        public CellConfig()
        {
            colorType = ColorType.None;
            keyId = -1;
            blockGroupId = -1;
        }
    }
}
