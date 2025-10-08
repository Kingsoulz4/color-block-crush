using System;
using System.Collections.Generic;

namespace ColorBlockCrush.Tools
{
    [Serializable]
    public class TunnelConfig
    {
        public int tankNumber;
        public List<TankConfig> tanks;
    }
}
