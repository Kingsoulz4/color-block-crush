using UnityEngine.UI;
using UnityEngine;
using System.Linq;

namespace ColorBlockCrush
{
    public class PopupStarterPack : PopupUI
    {
        [SerializeField] private ShopData m_list;
        
        [SerializeField] private Button m_buttonClose;
        [SerializeField] private Button m_buttonPurchaseStarterPack;
        
        private void Awake()
        {
            m_buttonClose.onClick.AddListener(OnClickClose);
            m_buttonPurchaseStarterPack.onClick.AddListener(OnClickPurchaseStarterPack);
        }
        
        private void OnClickPurchaseStarterPack()
        {
            var starterPack = m_list.listShopPack.Find(
                x => x.id == "starter_pack");
            if (starterPack == null) return;

            var popupReceiveRewards = UIManager.Instance.ShowPopup<PopupReceiveReward>(null);
            popupReceiveRewards.SetData(starterPack.listReward);
            foreach(var item in starterPack.listReward)
            {
                item.Claim();
            }
            ShopManager.Instance.HasPurchasedStarterPack = true;

            Hide();
        }

        private void OnClickClose()
        {
            Hide();
        }
    }
}
