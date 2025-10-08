using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush.Tools
{
    [Serializable]
    public class TankLineElementConfig
    {
        public TankLineElementType elementType;
        
        public TankConfig tankConfig;
        
        public TunnelConfig tunnelConfig;
    }
}
