using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ColorBlockCrush
{
    public class PopupOutOfSpace : PopupUI
    {
        [SerializeField] private Button m_buttonKeepPlaying;
        [SerializeField] private Button m_buttonClose;
        [SerializeField] private Text m_textPrice;
        
        [SerializeField] private ShopData shopData;
        [SerializeField] private FailOfferPack m_failOfferPack;

        private BoosterItemData boosterData;

        [SerializeField] private AudioClip failSfx;

        public Action OnKeepPlaying { get; set; }

        public Action OnClose { get; set; }

        private void Awake()
        {
            m_buttonClose.onClick.AddListener(OnClickClose);
            m_buttonKeepPlaying.onClick.AddListener(OnClickRevive);
        }

        public override void Show(Action onClose)
        {
            base.Show(onClose);
            boosterData = BoosterManager.Instance.BoosterData.GetBoosterItemData(BoosterType.REVIVAL);
            m_textPrice.text = boosterData.price.ToString();
            AudioManager.Instance.PlayOneShot(failSfx, 1);

            var pack = shopData.listShopPack.FirstOrDefault(x => x.id == "fail_offer");
            m_failOfferPack.SetData(pack, () =>
            {
                LevelController.Instance.ReviveLevel(boosterData.price);
                OnKeepPlaying?.Invoke();
                Hide();
            });
        }
        private void OnClickRevive()
        {
            if (UserDataManager.Gold >= boosterData.price)
            {
                LevelController.Instance.ReviveLevel(boosterData.price);
                OnKeepPlaying?.Invoke();
                Hide();
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
