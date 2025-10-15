using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ColorBlockCrush
{
    public class PopupOutOfSpace : PopupUI
    {
        [SerializeField] private Button m_buttonKeepPlaying;
        [SerializeField] private Button m_buttonClose;
        [SerializeField] private Text m_textPrice;

        private BoosterItemData boosterData; 

        public Action OnKeepPlaying { get; set; }

        public Action OnClose { get; set; }

        private void Awake()
        {
            m_buttonClose.onClick.AddListener(OnClickClose);
            m_buttonKeepPlaying.onClick.AddListener(OnClickKeepPlaying);
        }

        public void Show()
        {
            boosterData = BoosterManager.Instance.BoosterData.GetBoosterItemData(BoosterType.REVIVAL);
            m_textPrice.text = boosterData.price.ToString();
        }

        private void OnClickKeepPlaying()
        {
            if (UserDataManager.Gold >= boosterData.price)
            {
                UserDataManager.AddGold(-boosterData.price, "Revival");
                Hide();
                OnKeepPlaying?.Invoke();
            }
            else
            {
                UIManager.Instance.ShowPopup<PopupShop>(() =>
                {
                    GameManager.Instance.SetGameState(GameState.Playing);
                });
                GameManager.Instance.SetGameState(GameState.Paused);
            }
        }

        private void OnClickClose()
        {
            Hide();
            OnClose?.Invoke();
        }
    }
}
