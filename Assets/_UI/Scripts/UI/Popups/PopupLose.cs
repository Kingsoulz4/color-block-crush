using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ColorBlockCrush
{
    public class PopupLose : PopupUI
    {
        [SerializeField] private Button m_buttonRetry;
        [SerializeField] private Button m_buttonClose;
        [SerializeField] private Text m_textLevel;
        
        [SerializeField] private AudioClip failSfx;

        public Action OnRetry { get; set; }

        public Action OnClose { get; set; }

        private void OnEnable()
        {
            m_textLevel.text = $"Level {LevelManager.Instance.CurrentLevel}";
        }

        private void Awake()
        {
            m_buttonRetry.onClick.AddListener(OnClickRetry);
            m_buttonClose.onClick.AddListener(OnClickClose);
        }

        public override void Show(Action onClose)
        {
            base.Show(onClose);
            AudioManager.Instance.PlayOneShot(failSfx, 1);
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
