using System;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush.Tools
{
    [Serializable]
    public class GunConfig
    {
        [Header("Base Infor")]
        public ColorType colorType;
        public int bulletNumber;

        [Header("Lock")]
        public bool hasLock = false;
        
        [Header("Hidden")]
        public bool isHidden = false;
        
        [Header("Connect")]
        public List<int> gunConnect =  new List<int>();

        public GunConfig()
        {
            gunConnect = new List<int>();
        }

        public void ClearConnect()
        {
            if (gunConnect == null || gunConnect.Count == 0) return;
            
            gunConnect.Clear();
        }
    }
}
