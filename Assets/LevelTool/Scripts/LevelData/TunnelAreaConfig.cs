using System;
using System.Collections.Generic;

namespace ColorBlockCrush.Tools
{
    [Serializable]
    public class TunnelAreaConfig
    {
        public int tunnelAreaId;
        public int colorNumber;
        public List<TunnelAreaElementConfig> elements;
        public List<int> blocksId;

        public TunnelAreaConfig()
        {
            colorNumber = 0;
            elements = new List<TunnelAreaElementConfig>();
            blocksId = new List<int>();
        }
    }

    [Serializable]
    public class TunnelAreaElementConfig
    {
        public ColorType elementColor;
        public int health;
    }
}
