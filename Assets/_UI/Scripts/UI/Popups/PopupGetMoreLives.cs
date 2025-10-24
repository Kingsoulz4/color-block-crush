using System;
using System.Collections;
using System.Collections.Generic;
using Analytics;
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
                UserDataManager.AddHeart(5, "refill_heart", false, 0, ReasonType.exchange.ToString());
                UserDataManager.AddGold(-coinRequire, "refill_heart", ReasonType.exchange.ToString());
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
            Debug.Log("Claim Heart");
            MaxAdsManager.Instance.ShowRewardedAd(MaxKeys.rewardedID, () =>
            {
                UserDataManager.AddHeart(1, "refill_heart", false, reason: ReasonType.watch_ads.ToString());
            
                UIManager.Instance.ShowPopup<PopupReceiveHeart>(() =>
                {
                    Hide();
                    OnRefilled?.Invoke();
                }).PlayCollectFx(m_heartBar.transform.position, Vector3.zero, 1);
            });
        }
    }
}
