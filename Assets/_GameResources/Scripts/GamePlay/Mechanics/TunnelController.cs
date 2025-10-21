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
            CanShift = true;
            ColumnIndex = column;
            ID = id;    
        }

        public override void UpdateWhenColumnChange()
        {
            if(Index == 1)
            {
                CanShift = false;
            }    

            if (Index == 0)
            {
                m_animation.Play();
                LevelManager.Instance.LevelGame.GunBoardController.SpawnNewGun(ColumnIndex, 0, tunnelData.tanks[currentIndexGunSpawned], 0);
                currentIndexGunSpawned++;
            }

            if(currentIndexGunSpawned >= tunnelData.tanks.Count)
            {
                Resolve();
            }
        }
    }
}
