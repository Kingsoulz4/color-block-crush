using System;
using Analytics;
using UnityEngine;
using UnityEngine.UI;
using I2.Loc;
using Yoolax.Framework;

namespace ColorBlockCrush
{
    public class PopupBuyBooster : PopupUI
    {
        [SerializeField] private Button m_buttonClose;
        [SerializeField] private Button m_buttonBuy;
        [SerializeField] private Image m_imageBoosterIcon;
        [SerializeField] private Text m_textPrice;
        [SerializeField] private Text m_textTitle;
        [SerializeField] private Text m_textDes;

        public Action OnClose { get; set; }

        public Action OnBought { get; set; }

        private BoosterType boosterType;

        private void Awake()
        {
            m_buttonBuy.onClick.AddListener(OnClickBuy);
            m_buttonClose.onClick.AddListener(OnClickClose);
        }

        public void Show(BoosterType boosterType)
        {
            this.boosterType = boosterType;
            var boosterData = BoosterManager.Instance.BoosterData.GetBoosterItemData(boosterType);
            var iconSprite = boosterData.icon;
            m_imageBoosterIcon.sprite = iconSprite;
            m_textPrice.text = boosterData.price + "";
            m_textTitle.text = LocalizationManager.GetTranslation(boosterData.title);
            m_textDes.text = LocalizationManager.GetTranslation(boosterData.description);
        }

        private void OnClickClose()
        {
            Hide();
            OnClose?.Invoke();
        }

        private void OnClickBuy()
        {
            Hide();
            var boosterData = BoosterManager.Instance.BoosterData.GetBoosterItemData(boosterType);
            if (UserDataManager.Gold >= boosterData.price)
            {
                UserDataManager.AddBooster(boosterType, 3);
                UserDataManager.AddGold(-boosterData.price, $"buy_{boosterType.ToString().ToLower()}",
                    ReasonType.exchange.ToString());
                
                string[] types = { ResourceType.booster.ToString()};
                string[] names = { boosterData.boosterType.ToString().ToLower()};
                string[] amounts = {"1"};
                ResourceAnalyticStruct resourceAnalyticStruct = new ResourceAnalyticStruct(types, names, amounts, $"buy_{boosterType.ToString().ToLower()}",
                    ReasonType.exchange.ToString());
                Server.Get<OnResourceEarnEventLog>().Dispatch(resourceAnalyticStruct);

                resourceAnalyticStruct = new ResourceAnalyticStruct(types, names, amounts, $"use_{boosterType.ToString().ToLower()}",
                    ReasonType.use.ToString());
                Server.Get<OnResourceSpendEventLog>().Dispatch(resourceAnalyticStruct);
                OnBought?.Invoke();
            }
            else
            {
                OnClose?.Invoke();
                UIManager.Instance.ShowPopup<PopupShop>(() =>
                {
                    GameManager.Instance.SetGameState(GameState.Playing);
                });
                GameManager.Instance.SetGameState(GameState.Paused);
            }
        }
    }
}