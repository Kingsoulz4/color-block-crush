using System;
using System.Collections.Generic;

namespace ColorBlockCrush.Tools
{
    [Serializable]
    public class KeyConfig
    {
        public int keyId;
        public List<int> cellsId;

        public KeyConfig()
        {
            cellsId = new List<int>();
        }
    }
}
