using System;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using Analytics;
using Yoolax.Framework;

namespace ColorBlockCrush
{
    public class PopupStarterPack : PopupUI
    {
        [SerializeField] private ShopData m_list;
        
        [SerializeField] private Button m_buttonClose;
        [SerializeField] private Button m_buttonPurchaseStarterPack;
        [SerializeField] private GoldDisplay m_goldBar;

        [SerializeField] private Text coinReceive;
        [SerializeField] private Text priceValueTxt;

        private Action OnPurchase;
        
        private void Awake()
        {
            m_buttonClose.onClick.AddListener(OnClickClose);
            m_buttonPurchaseStarterPack.onClick.AddListener(OnClickPurchaseStarterPack);
            
            var starterPack = m_list.listShopPack.Find(
                x => x.id == "starter_pack");
            if (starterPack == null) return;
            var goldQuantity = starterPack.listReward.Find(x => x.type == ItemType.GOLD).quantity;
            coinReceive.text = goldQuantity.ToString();
            priceValueTxt.text = IAPManager.Instance.GetLocalizedPriceString(starterPack.id);
        }

        private void OnEnable()
        {
            InAppPurchaseAnalyticStruct iapAnalyticStruct = new InAppPurchaseAnalyticStruct();
            iapAnalyticStruct = iapAnalyticStruct.SetBaseIAP(UiHolderManager.Instance.GetCurrentPlacement(), IAPShowType.pack, AnalyticManager.Instance.iAPTriggerType, "StarterPack")
                .SetIAPShow();
            Server.Get<OnIAPShowEventLog>().Dispatch(iapAnalyticStruct);
        }

        private void OnClickPurchaseStarterPack()
        {
            var starterPack = m_list.listShopPack.Find(
                x => x.id == "starter_pack");
            if (starterPack == null) return;
            
            var popupLoadingProcess = UIManager.Instance.ShowPopup<PopupLoadingProcess>(null);
            popupLoadingProcess.ShowPopup();
            
            IAPManager.Instance.BuyProductID(starterPack.id, (success) =>
            {
                popupLoadingProcess.Hide();
                if (success)
                {
                    var popupReceiveRewards = UIManager.Instance.ShowPopup<PopupReceiveReward>(null);
                    popupReceiveRewards.SetData(starterPack.listReward);
                    
                    string[] types = new string[starterPack.listReward.Count];
                    string[] names = new string[starterPack.listReward.Count];
                    string[] amounts = new string[starterPack.listReward.Count];
                    for(int i = 0; i < starterPack.listReward.Count; i++)
                    {
                        starterPack.listReward[i].Claim();
                        types[i] = AnalyticUtils.GetCurrencyTypeFromCurrencyName(starterPack.listReward[i].type.ToString().ToLower()).ToString();
                        names[i] = AnalyticUtils.GetNameFromType(starterPack.listReward[i].type.ToString().ToLower());
                        amounts[i] = starterPack.listReward[i].quantity.ToString();
                    }
                    ResourceAnalyticStruct resourceAnalyticStruct = new ResourceAnalyticStruct(types, names, amounts, starterPack.title.ToLower(),
                        ReasonType.purchase.ToString());
                    Server.Get<OnResourceEarnEventLog>().Dispatch(resourceAnalyticStruct);
            
                    m_goldBar.gameObject.SetActive(false);
                    var popupReceiveCoin = UIManager.Instance.ShowPopup<PopupReceiveCoin>(null);
                    var goldQuantity = starterPack.listReward.Find(x => x.type == ItemType.GOLD).quantity;
                    popupReceiveCoin.PlayCoinFX(m_goldBar.transform.position, Vector3.zero, goldQuantity, () =>
                    {
                        m_goldBar.gameObject.SetActive(true);
                        m_goldBar.SetText(UserDataManager.Gold);
                        Hide();
                    });
                    ShopManager.Instance.HasPurchasedStarterPack = true;
                }
                else
                {
                    UIManager.Instance.ShowPopup<PopupPurchaseFailed>(null);
                }
            });
        }

        private void OnClickClose()
        {
            InAppPurchaseAnalyticStruct iapAnalyticStruct = new InAppPurchaseAnalyticStruct();       
            iapAnalyticStruct = iapAnalyticStruct.SetBaseIAP(AnalyticManager.Instance.GetCurrentPrefixPlacement() + UiHolderManager.Instance.GetCurrentPlacement(), IAPShowType.pack, AnalyticManager.Instance.iAPTriggerType, "StarterPack")
                .SetIAPClose(Time.time - AnalyticManager.Instance.timeOpenPopupIap);
            Server.Get<OnIAPCloseEventLog>().Dispatch(iapAnalyticStruct);
            Hide();
        }
    }
}
