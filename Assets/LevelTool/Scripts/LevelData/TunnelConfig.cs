using System;
using System.Collections.Generic;

namespace Geckout.Tools
{
    [Serializable]
    public class TunnelConfig
    {
        public int tankNumber;
        public List<TankConfig> tanks;
    }
}
