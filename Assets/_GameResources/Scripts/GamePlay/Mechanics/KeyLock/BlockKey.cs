using ColorBlockCrush.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush
{
    public class BlockKey : MonoBehaviour
    {
        private KeyConfig keyData;
        private LevelConfig levelData;

        public void Init(LevelConfig levelData, KeyConfig keyData)
        {
            this.keyData = keyData;
            this.levelData = levelData;
            
        }     
    }
}
