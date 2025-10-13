using System;
using System.Collections.Generic;

namespace ColorBlockCrush.Tools
{
    [Serializable]
    public class TunnelConfig
    {
        public int tankNumber;
        public List<GunConfig> tanks;

        public TunnelConfig()
        {
            tankNumber = 0;
            tanks = new List<GunConfig>();
        }
    }
}
