using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Geckout.Tools
{
    [Serializable]
    public class CellConfig
    {
        [Header("Base Infor")]
        public int id;
        public Vector2Int coordinate;
        public ColorType colorType;

        [Header("Key")]
        public bool isKey = false;

        [Header("Block")] 
        public bool isBlock = false;
        public int blockHealth;
        public int blockGroupId;
    }
}
