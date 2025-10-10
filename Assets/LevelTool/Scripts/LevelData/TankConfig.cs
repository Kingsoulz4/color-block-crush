using System;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush.Tools
{
    [Serializable]
    public class TankConfig
    {
        [Header("Base Infor")]
        public ColorType colorType;
        public int bulletNumber;

        [Header("Lock")]
        public bool hasLock = false;
        
        [Header("Hidden")]
        public bool isHidden = false;
        
        [Header("Connect")]
        public List<int> tankConnect =  new List<int>();

        public void ClearConnect()
        {
            if (tankConnect == null || tankConnect.Count == 0) return;
            
            tankConnect.Clear();
        }
    }
}
