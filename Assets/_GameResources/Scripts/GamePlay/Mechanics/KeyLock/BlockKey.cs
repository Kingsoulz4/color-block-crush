using ColorBlockCrush.Tools;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush
{
    public class BlockKey : MonoBehaviour
    {
        private KeyConfig keyData;

        private List<Block> listBlock = new();

        public bool IsUnBlocked { get; set; } = false;

        public bool IsResolved { get; private set; } = false;

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
            //var lastBlock = listBlock[^1];
            for (int i=0; i<listBlock.Count; i++)
            {

                if (listBlock[i].IsDestroyed)
                {
                    Resolve();
                    return;
                }
    
                //lastBlock = listBlock[i];
            }
        }

        public void Resolve()
        {
            Debug.Log("Key Resolved");

            IsUnBlocked = true;

            if (IsResolved) return;

            var pendingLock = LevelManager.Instance.LevelGame.GunBoardController.GetPenndingLock();
            if (pendingLock != null)
            {
                IsResolved = true;
                transform.DOMove(pendingLock.transform.position, 0.25f).OnComplete(() =>
                {
                    Destroy(gameObject);
                });
                pendingLock.Resolve();
            }
        }
            
    }
}
