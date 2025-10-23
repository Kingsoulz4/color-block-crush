using System;
using Analytics;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Yoolax.Framework;

namespace ColorBlockCrush
{
    public class BundlePack : MonoBehaviour
    {
        [SerializeField] private ItemRewardUI m_itemRewardPrefab;
        [SerializeField] private Image m_iconCoin;
        [SerializeField] private Transform m_listRewardContainer;
        [SerializeField] private Button m_buttonBuy;
        [SerializeField] private Text m_textPrice;
        [SerializeField] private Text m_textCoinQuantity;
        [SerializeField] private Text m_textPackName;
        [SerializeField] private GameObject m_tagHighlight;
        [SerializeField] private string packIDHighlight;

        private ShopPack shopPack;
        public Action OnPurchased { get; set; }

        private void Awake()
        {
            m_buttonBuy.onClick.AddListener(OnClickBuy);
        }

        private void OnClickBuy()
        {
            if(shopPack == null) return;
            
            // var popupLoadingProcess = UIManager.Instance.ShowPopup<PopupLoadingProcess>(null);
            // popupLoadingProcess.ShowPopup();
            IAPManager.Instance.BuyProductID(shopPack.id, (success) =>
            {
                //popupLoadingProcess.Hide();
                if (success)
                {
                    
                    ShopManager.Instance.AddPurchasedPack(shopPack);
                    var popupReceiveReward = UIManager.Instance.ShowPopup<PopupReceiveReward>(() =>
                    {
                        OnPurchased?.Invoke();
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
                    // transform.DOScale(0, 0.1f).OnComplete(() =>
                    // {
                    //     Destroy(gameObject);
                    // });
                }
                else
                {
                    UIManager.Instance.ShowPopup<PopupPurchaseFailed>(null);
                }
            });
        }

        public void SetData(ShopPack packData)
        {
            this.shopPack = packData;
            m_textCoinQuantity.text = packData.listReward.Find(x => x.type == ItemType.GOLD).quantity + "";
            m_iconCoin.sprite = packData.icon;
            m_textPackName.text = packData.title;
            m_tagHighlight.gameObject.SetActive(packData.id == packIDHighlight);
            m_textPrice.text = IAPManager.Instance.GetLocalizedPriceString(packData.id);
            MyUlti.RemoveAllChilds(m_listRewardContainer);
            foreach(var item in packData.listReward.Where(x => x.type != ItemType.GOLD))
            {
                var newItem = Instantiate(m_itemRewardPrefab, m_listRewardContainer);
                newItem.gameObject.SetActive(true);
                newItem.SetData(item);
            }

        }
    }
}
