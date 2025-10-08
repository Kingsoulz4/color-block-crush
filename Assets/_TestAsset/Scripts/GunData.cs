using System;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush
{

    [Serializable]
    public class GunColumnData
    {
        public List<GunData> Guns = new List<GunData>();
    }

    [Serializable]
    public class GunData
    {
        public ColorType Color;
        public int BulletCount;

        public GunData(ColorType color, int bulletCount)
        {
            Color = color;
            BulletCount = bulletCount;
        }
    }
}