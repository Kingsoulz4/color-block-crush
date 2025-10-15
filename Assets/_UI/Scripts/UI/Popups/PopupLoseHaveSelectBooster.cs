using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace ColorBlockCrush
{
    public class PopupLoseHaveSelectBooster : PopupUI
    {
        [SerializeField] private Button m_buttonRetry;
        [SerializeField] private Button m_buttonClose;
        [SerializeField] private Text m_textLevel;

        public Action OnRetry { get; set; }

        public Action OnClose { get; set; }

        private void OnEnable()
        {
            m_textLevel.text = $"Level {LevelManager.Instance.CurrentLevel}";
            UpdateUI();
        }

        private void Awake()
        {
            m_buttonRetry.onClick.AddListener(OnClickRetry);
            m_buttonClose.onClick.AddListener(OnClickClose);
        }

        private void UpdateUI()
        {
            m_textLevel.text = $"Level {LevelManager.Instance.CurrentLevel}";
        }

        private void OnClickClose()
        {
            Hide();
            OnClose?.Invoke();
        }

        private void OnClickRetry()
        {
            Hide();
            OnRetry?.Invoke();
        }
    }
}
