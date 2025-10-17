using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;

namespace ColorBlockCrush.Tools
{
    public class ItemTunnelAreaQueueView : ItemQueueViewBase
    {
        public RectTransform rectTransform;
        public Image IconColor;
        public TunnelAreaElementConfig elementConfig;
        public TextMeshProUGUI healthNumber;
        public Button buttonDelete;

        private Action<int, int> onChangeIndex;

        public override void Init(Action<int> onDetete = null, Action<int, int> onChangeIndex = null)
        {
            base.Init(onDetete, onChangeIndex);
            buttonDelete.onClick.AddListener(() => {
                onDetete?.Invoke(transform.GetSiblingIndex());
            });
        }
        
    }
}
