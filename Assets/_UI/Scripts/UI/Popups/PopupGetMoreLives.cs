using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ColorBlockCrush
{
    public class PopupGetMoreLives : PopupUI
    {
        [SerializeField] private Button m_buttonClaimByCoin;
        [SerializeField] private Button m_buttonClaimByAds;
        [SerializeField] private Button m_buttonClose;
        
        [SerializeField] private Text m_textHeartQuantity;
        [SerializeField] private Text m_textCountDown;
        [SerializeField] private Text m_textPriceCoinRefill;

        [SerializeField] private HeartDisplay m_heartBar;

        private int coinRequire;

        public Action OnClose { get; set; }

        public Action OnRefilled { get; set; }

        private void Awake()
        {
            m_buttonClaimByAds.onClick.AddListener(OnClickClaimByAds);
            m_buttonClaimByCoin.onClick.AddListener(OnClickClaimByCoin);
            m_buttonClose.onClick.AddListener(OnClickClose);
        }
        
        private void OnEnable()
        {
            m_textHeartQuantity.text = UserDataManager.Heart.ToString();
            var boosterData = BoosterManager.Instance.BoosterData.GetBoosterItemData(BoosterType.LIVES);
            coinRequire = boosterData.price;
            m_textPriceCoinRefill.text = coinRequire.ToString();
        }
        
        private void Update()
        {
            m_textCountDown.text = HeartManager.Instance.GetTimeRemaningText();
        }

        private void OnClickClose()
        {
            Hide();
            OnClose?.Invoke();
        }

        private void OnClickClaimByCoin()
        {
            if (UserDataManager.Gold >= coinRequire)
            {
                int heartNeedToFull = 5 - UserDataManager.Heart;
                UserDataManager.AddHeart(5, "Refill Heart", false);
                UserDataManager.AddGold(-coinRequire, "Refill Heart");
                UIManager.Instance.ShowPopup<PopupReceiveHeart>(() => {
                    Hide();
                    OnRefilled?.Invoke();
                }).PlayCollectFx(m_heartBar.transform.position, Vector3.zero, heartNeedToFull);
            }
            else
            {
                OnClose?.Invoke();
                UIManager.Instance.ShowPopup<PopupShop>(null);
            }
        }

        private void OnClickClaimByAds()
        {
            UserDataManager.AddHeart(1, "Refill Heart", false);
            
            UIManager.Instance.ShowPopup<PopupReceiveHeart>(() =>
            {
                Hide();
                OnRefilled?.Invoke();
            }).PlayCollectFx(m_heartBar.transform.position, Vector3.zero, 1);
        }
    }
}
