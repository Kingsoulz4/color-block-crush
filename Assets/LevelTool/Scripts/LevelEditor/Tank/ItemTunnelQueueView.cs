using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

namespace  ColorBlockCrush.Tools
{
    public class ItemTunnelQueueView : MonoBehaviour
    {
        public RectTransform rectTransform;
        public Image IconColor;
        public GunConfig tankConfig;
        public TextMeshProUGUI bulletNumber;
        public Button buttonDelete;

        private Action<int, int> onChangeIndex;

        public void Init(Action<int> onDetete = null, Action<int, int> onChangeIndex = null)
        {
            buttonDelete.onClick.AddListener(() => {
                onDetete?.Invoke(transform.GetSiblingIndex());
            });
            this.onChangeIndex = onChangeIndex;
        }

        public void ChangeIndex(int oldIndex, int newIndex)
        {
            onChangeIndex?.Invoke(oldIndex, newIndex);
        }

    }
}
