using System;
using UnityEngine;

namespace ColorBlockCrush.Tools
{
    public abstract class ItemQueueViewBase : MonoBehaviour
    {
        protected Action<int, int> onChangeIndex;

        public virtual void Init(Action<int> onDetete = null, Action<int, int> onChangeIndex = null)
        {
            this.onChangeIndex = onChangeIndex;
        }

        public virtual void ChangeIndex(int oldIndex, int newIndex)
        {
            onChangeIndex?.Invoke(oldIndex, newIndex);
        }
    }
}
