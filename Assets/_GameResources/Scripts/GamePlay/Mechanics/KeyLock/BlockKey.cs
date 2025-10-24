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
                StartCoroutine(IEResolve(pendingLock));
                pendingLock.Resolve();
            }
        }

        public void Resolve(LockObject lockObject)
        {
            Debug.Log("Key Resolved");

            IsUnBlocked = true;

            if (IsResolved) return;

            var pendingLock = lockObject;
            if (pendingLock != null)
            {
                IsResolved = true;
                StartCoroutine(IEResolve(pendingLock));
                pendingLock.Resolve();
            }
        }

        private IEnumerator IEResolve(LockObject pendingLock)
        {
            float timeEachStep = 0.25f;
            transform.DOMove(transform.position + Vector3.up * 5, timeEachStep);
            transform.DOScale(4, timeEachStep);
            transform.DOLocalRotate(new Vector3(90, 0, 0), timeEachStep);
            yield return new WaitForSeconds(timeEachStep);
            transform.DOMove(pendingLock.transform.position + Vector3.up * 0.25f - Vector3.forward * 0.075f, timeEachStep);
            transform.DOScale(2, timeEachStep);
            transform.DOLocalRotate(new Vector3(0, 90, 0), timeEachStep);
            yield return new WaitForSeconds(timeEachStep);
            yield return new WaitForSeconds(0.1f);
            transform.DOLocalRotate(new Vector3(0, 0, 0), timeEachStep);
            yield return new WaitForSeconds(timeEachStep);
            transform.DOScale(0, timeEachStep).OnComplete(() =>
            {
                Destroy(gameObject);
            });
        }
            
            
            
    }
}
