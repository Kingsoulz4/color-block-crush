using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush
{
    public class LockObject : ObjectOnGunBoardColumn
    {
        public bool IsResolved { get; private set; }

        public void Resolve()
        {
            if (IsResolved) return;

            IsResolved = true;

            transform.DOScale(0, 0.25f)
                .OnComplete(() =>
                {
                    LevelManager.Instance.LevelGame.GunBoardController.ResolveLock(this);
                });
        } 

        internal void Init(int column)
        {
            ColumnIndex = column;
        }

        public override void UpdateWhenColumnChange()
        {
            if (Index == 0)
            {
                var pendingKey = LevelManager.Instance.LevelGame.BlockBoardController.GetPendingKey();
                if (pendingKey != null)
                {
                    pendingKey.Resolve(this);
                }
            }
        }
    }
}
