using ColorBlockCrush.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush
{
    public class BlockKey : MonoBehaviour
    {
        private KeyConfig keyData;

        private List<Block> listBlock = new();

        public void Init(KeyConfig keyData)
        {
            this.keyData = keyData;
        }     

        public void AddBlock(Block block)
        {
            if(!listBlock.Contains(block) && !keyData.blockId.Contains(block.Id))
            {
                listBlock.Add(block);
            }
            
        }

        public void CheckCanResolve()
        {
            var lastBlock = listBlock[^1];
            for (int i=0; i<listBlock.Count; i++)
            {
                if(lastBlock == null && listBlock[i] == null)
                {
                    Resolve();
                }
                lastBlock = listBlock[i];
            }
        }

        public void Resolve()
        {
            Debug.Log("Key Resolved");
            var pendingLock = LevelManager.Instance.LevelGame.GunBoardController.GetPenndingLock();
            if (pendingLock != null)
            {
                pendingLock.Resolve();
            }
        }
            
    }
}
