using System;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

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

                    foreach(var item in shopPack.listReward)
                    {
                        item.Claim();
                    }
                }
                else
                {
                    UIManager.Instance.ShowPopup<PopupPurchaseFailed>(null);
                }
            });
        }
    }
}
