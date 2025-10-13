using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush.Tools
{
    [Serializable]
    public class BlockConfig
    {
        [Header("Base Infor")]
        public int id;
        public Vector2Int coordinate;
        public ColorType colorType;
        public BlockType blockType;

        [Header("Key")]
        public int keyId;

        [Header("Block")] 
        public int bigBlockId;

        public BlockConfig()
        {
            colorType = ColorType.None;
            keyId = -1;
            bigBlockId = -1;
        }
    }
}
