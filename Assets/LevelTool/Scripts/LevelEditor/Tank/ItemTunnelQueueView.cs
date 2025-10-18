using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

namespace ColorBlockCrush.Tools
{
    public class ItemTunnelQueueView : ItemQueueViewBase
    {
        public RectTransform rectTransform;
        public Image IconColor;
        public GunConfig tankConfig;
        public TextMeshProUGUI bulletNumber;
        public Button buttonDelete;

        private Action<int, int> onChangeIndex;

        public override void Init(Action<int> onDetete = null, Action<int, int> onChangeIndex = null)
        {
            base.Init(onDetete, onChangeIndex);
            buttonDelete.onClick.AddListener(() => { onDetete?.Invoke(transform.GetSiblingIndex()); });
            
        }
    }
}