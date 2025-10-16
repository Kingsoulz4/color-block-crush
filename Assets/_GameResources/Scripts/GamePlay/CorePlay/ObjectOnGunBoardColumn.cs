using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush
{
    public class ObjectOnGunBoardColumn : MonoBehaviour
    {
        public int ColumnIndex { get; set; }

        public int Index { get; private set; } = 0;

        protected Tween moveSortSlotTw;

        public virtual void SetIndex(int index)
        {
            Index = index;
        }

        public virtual void UpdateWhenColumnChange()
        {
           
        }

        public virtual void MoveColumn(Vector3 targetPos, float _shiftDuration, Ease _shiftEase)
        {
            Sequence moveSortSlotSq = DOTween.Sequence();
            moveSortSlotTw = moveSortSlotSq.Append(transform.DOMove(targetPos, _shiftDuration).SetEase(_shiftEase)).OnComplete(() =>
            {
            });
            moveSortSlotSq.SetId(this);
        }
    }
}
