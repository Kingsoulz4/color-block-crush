using ColorBlockCrush.Tools;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush
{
    public class TunnelController : ObjectOnGunBoardColumn
    {
        [SerializeField] private Animation m_animation;

        public int ID { get; set; }
        public bool IsResolved { get; private set; }

        private TunnelConfig tunnelData;

        private int currentIndexGunSpawned = 0;

        private bool isFirstUpdate = false;

        public void Resolve()
        {
            if (IsResolved) return;

            IsResolved = true;

            transform.DOScale(0, 0.25f)
                .OnComplete(() =>
                {
                    LevelManager.Instance.LevelGame.GunBoardController.ResolveTunnel(this);
                });

        }

        internal void Init(TunnelConfig tunnelConfig, int column ,int id)
        {
            tunnelData = tunnelConfig;
            CanShift = false;
            ColumnIndex = column;
            ID = id;    
        }

        public override void UpdateWhenColumnChange()
        {
            if(!isFirstUpdate)
            {
                isFirstUpdate = true;
                return;
            }    

            m_animation.Play();
            DOVirtual.DelayedCall(0.5f, () =>
            {
                LevelManager.Instance.LevelGame.GunBoardController.SpawnNewGun(ColumnIndex, Index, tunnelData.tanks[currentIndexGunSpawned], 0);
            });
            currentIndexGunSpawned++;

            if(currentIndexGunSpawned >= tunnelData.tanks.Count)
            {
                Resolve();
            }
        }
    }
}
