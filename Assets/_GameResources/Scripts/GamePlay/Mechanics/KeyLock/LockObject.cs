using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush
{
    public class LockObject : ObjectOnGunBoardColumn
    {
        [SerializeField] private Animator m_animator;

        public bool IsResolved { get; private set; }

        public void Resolve()
        {
            if (IsResolved) return;

            IsResolved = true;

            transform.DOScale(0, 0.25f)
                .OnStart(() =>
                {
                    m_animator.Play("LockUnlock");
                })
                .OnComplete(() =>
                {
                    LevelManager.Instance.LevelGame.GunBoardController.ResolveLock(this);
                })
                .SetDelay(0.85f);
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
