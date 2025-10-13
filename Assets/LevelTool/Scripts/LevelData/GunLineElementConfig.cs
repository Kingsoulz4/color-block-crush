using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush.Tools
{
    [Serializable]
    public class GunLineElementConfig
    {
        public int elementId;
        
        public GunLineElementType elementType;
        
        public GunConfig gunConfig;
        
        public TunnelConfig tunnelConfig;
    }
}
