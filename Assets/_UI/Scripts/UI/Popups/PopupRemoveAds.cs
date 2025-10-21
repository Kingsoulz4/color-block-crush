using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ColorBlockCrush
{
    public class PopupRemoveAds : PopupUI
    {
        [SerializeField] private ShopData m_listRemoveAdsPacks;

        [SerializeField] private Button m_buttonClose;
        [SerializeField] private Button m_buttonPurchaseNoAdsPack;
        
        [SerializeField] private GameObject m_removeAdsPack;

        [SerializeField] private Text priceValueTxt;

        private void Awake()
        {
            m_buttonClose.onClick.AddListener(OnClickClose);
            m_buttonPurchaseNoAdsPack.onClick.AddListener(OnClickPurchaseNoAdsPack);
            var packNoAds = m_listRemoveAdsPacks.listShopPack.Find(
                x => x.listReward.FirstOrDefault(y => y.type == ItemType.REMOVE_ADS) != null);
            priceValueTxt.text = IAPManager.Instance.GetLocalizedPriceString(packNoAds.id);
        }

        public void UpdateUI()
        {
            m_removeAdsPack.gameObject.SetActive(!ShopManager.Instance.HasPurchasedNoAdsPack);
        }
        
        private void OnClickPurchaseNoAdsPack()
        {
            var packNoAds = m_listRemoveAdsPacks.listShopPack.Find(
               x => x.listReward.FirstOrDefault(y => y.type == ItemType.REMOVE_ADS) != null);
            if (packNoAds == null) return;
            
            IAPManager.Instance.BuyProductID(packNoAds.id, (success) =>
            {
                if (success)
                {
                    var popupReceiveRewards = UIManager.Instance.ShowPopup<PopupReceiveReward>(null);
                    popupReceiveRewards.SetData(packNoAds.listReward);
                    ShopManager.Instance.HasPurchasedNoAdsPack = true;
                    Hide();
                }
                else
                {
                    UIManager.Instance.ShowPopup<PopupPurchaseFailed>(null);
                }
            });
        }

        private void OnClickClose()
        {
            Hide();
        }
    }
}
