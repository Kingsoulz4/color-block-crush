using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush.Tools
{
    [Serializable]
    public class PixelSnakeConfig
    {
        public int pixelSnakeId;
        public ColorType colorType;
        public int health;
        public List<int> blocksId;

        public PixelSnakeConfig()
        {
            blocksId = new List<int>();
        }
    }
}
