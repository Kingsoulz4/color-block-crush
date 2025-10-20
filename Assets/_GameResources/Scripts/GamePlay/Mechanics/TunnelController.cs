using ColorBlockCrush.Tools;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush
{
    public class TunnelController : ObjectOnGunBoardColumn
    {
        public bool IsResolved { get; private set; }

        private TunnelConfig tunnelData;

        private int currentIndexGunSpawned = 0;

        public void Resolve()
        {
            if (IsResolved) return;

            IsResolved = true;

            transform.DOScale(0, 0.25f)
                .OnComplete(() =>
                {
                    
                });

        }

        internal void Init(TunnelConfig tunnelConfig)
        {
            tunnelData = tunnelConfig;
            CanShift = false;
        }

        public override void UpdateWhenColumnChange()
        {
            if (Index == 1)
            {
                LevelManager.Instance.LevelGame.GunBoardController.SpawnNewGun(0, ColumnIndex, tunnelData.tanks[currentIndexGunSpawned], 0);
                currentIndexGunSpawned++;
            }

            if(currentIndexGunSpawned >= tunnelData.tanks.Count)
            {
                Resolve();
            }
        }
    }
}
