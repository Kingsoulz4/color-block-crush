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

        [Header("Big Block")] 
        public int bigBlockId;
        
        [Header("Tunnel Area")]
        public int tunnelAreaId;
        
        [Header("Pixel Snake")]
        public int pixelSnakeId;

        public BlockConfig()
        {
            colorType = ColorType.None;
            keyId = -1;
            bigBlockId = -1;
            tunnelAreaId = -1;
            pixelSnakeId = -1;
        }
    }
}
