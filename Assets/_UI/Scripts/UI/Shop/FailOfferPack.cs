using System;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using Analytics;
using Yoolax.Framework;

namespace ColorBlockCrush
{
    public class FailOfferPack : MonoBehaviour
    {
        [SerializeField] private Button m_buttonBuy;
        [SerializeField] private Text m_textPrice;
        [SerializeField] private Text m_textCoinQuantity;
        [SerializeField] private Text m_textPackName;
        
        private ShopPack shopPack;
        private Action onRevival;
        public Action OnPurchased { get; set; }
        
        private void Awake()
        {
            m_buttonBuy.onClick.AddListener(OnClickBuy);
        }

        private void OnEnable()
        {
            InAppPurchaseAnalyticStruct iapAnalyticStruct = new InAppPurchaseAnalyticStruct();
            AnalyticManager.Instance.iAPShow = IAPShowType.pack;
            AnalyticManager.Instance.iAPTriggerType = TriggerType.popup;
            iapAnalyticStruct = iapAnalyticStruct.SetBaseIAP(AnalyticManager.Instance.GetCurrentPrefixPlacement() + UiHolderManager.Instance.GetCurrentPlacement(),
                AnalyticManager.Instance.iAPShow, AnalyticManager.Instance.iAPTriggerType, "fail_offer").SetIAPShow();
            Server.Get<OnIAPShowEventLog>().Dispatch(iapAnalyticStruct);
        }

        public void SetData(ShopPack packData, Action onRevival)
        {
            this.shopPack = packData;
            m_textCoinQuantity.text = packData.listReward.Find(x => x.type == ItemType.GOLD).quantity + "";
            m_textPackName.text = packData.title;
            this.onRevival = onRevival;
            m_textPrice.text = IAPManager.Instance.GetLocalizedPriceString(packData.id);
        }
        
        private void OnClickBuy()
        {
            if (shopPack == null)
            {
                UIManager.Instance.ShowPopup<PopupPurchaseFailed>(null);
                return;
            }

            var popupLoadingProcess = UIManager.Instance.ShowPopup<PopupLoadingProcess>(null);
            popupLoadingProcess.ShowPopup();
            IAPManager.Instance.BuyProductID(shopPack.id, (success) =>
            {
                popupLoadingProcess.Hide();
                if (success)
                {
                    ShopManager.Instance.AddPurchasedPack(shopPack);
                    var popupReceiveReward = UIManager.Instance.ShowPopup<PopupReceiveReward>(() =>
                    {
                        OnPurchased?.Invoke();
                        onRevival?.Invoke();
                    });
                    popupReceiveReward.SetData(shopPack.listReward);

                    string[] types = new string[shopPack.listReward.Count];
                    string[] names = new string[shopPack.listReward.Count];
                    string[] amounts = new string[shopPack.listReward.Count];
                    for(int i = 0; i < shopPack.listReward.Count; i++)
                    {
                        shopPack.listReward[i].Claim();
                        types[i] = AnalyticUtils.GetCurrencyTypeFromCurrencyName(shopPack.listReward[i].type.ToString().ToLower()).ToString();
                        names[i] = AnalyticUtils.GetNameFromType(shopPack.listReward[i].type.ToString().ToLower());
                        amounts[i] = shopPack.listReward[i].quantity.ToString();
                    }
                    ResourceAnalyticStruct resourceAnalyticStruct = new ResourceAnalyticStruct(types, names, amounts, shopPack.title.ToLower(),
                        ReasonType.purchase.ToString());
                    Server.Get<OnResourceEarnEventLog>().Dispatch(resourceAnalyticStruct);
                }
                else
                {
                    UIManager.Instance.ShowPopup<PopupPurchaseFailed>(null);
                }
            });
        }
    }
}
