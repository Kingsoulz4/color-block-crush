using System;
using UnityEngine.UI;
using UnityEngine;

namespace ColorBlockCrush
{
    public class PopupPurchaseFailed : PopupUI
    {
        [SerializeField] private Button m_buttonOk;

        private void Awake()
        {
            m_buttonOk.onClick.AddListener(Hide);
        }
    }
}
