using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush.Tools
{
    public class TankConfig : MonoBehaviour
    {
        [Header("Base Infor")]
        public int id;
        public ColorType colorType;
        public int bulletNumber;

        [Header("Lock")]
        public bool hasLock = false;
        
        [Header("Connect")]
        public List<int> tankConnect =  new List<int>();
    }
}
