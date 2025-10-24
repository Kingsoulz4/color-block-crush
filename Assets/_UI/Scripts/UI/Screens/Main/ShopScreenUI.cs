using System;
using Analytics;
using Yoolax.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace ColorBlockCrush
{
    public class ShopScreenUI : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private BundlePack m_bundlePackPrefab;
        [SerializeField] private CoinPack m_coinPackPrefab;

        [Header("Data")]
        [SerializeField] private ShopData m_listBundlePacks;
        [SerializeField] private ShopData m_listCoinPacks;

        [Header("Containers")]
        [SerializeField] private Transform m_listShopCoinPackContainer;
        [SerializeField] private Transform m_listPackContainer;
        [SerializeField] private Transform m_listBundlePackContainer;

        [Header("Others Packs")]
        [SerializeField] private Button m_buttonRemoveAdsPacks;
        [SerializeField] private GameObject m_removeAdsPacks;
        [SerializeField] private FreeCoinPack m_freeCoinPack;

        [Header("UI")]
        [SerializeField] private GoldDisplay m_goldBar;

        private void Awake()
        {
            m_buttonRemoveAdsPacks.onClick.AddListener(OnClickRemoveAdPacks);

            Init();
        }

        private void OnClickRemoveAdPacks()
        {
            UIManager.Instance.ShowPopup<PopupRemoveAds>(null);
        }

        private void OnEnable()
        {
            InAppPurchaseAnalyticStruct iapAnalyticStruct = new InAppPurchaseAnalyticStruct();
            iapAnalyticStruct = iapAnalyticStruct.SetBaseIAP(UiHolderManager.Instance.GetCurrentPlacement(), IAPShowType.shop, TriggerType.click, "Null")
                .SetIAPShow();
            Server.Get<OnIAPShowEventLog>().Dispatch(iapAnalyticStruct);
            Server.Get<OnBuyNoAds>().AddListener(UpdateUI);
            UpdateUI();
        }

        private void OnDisable()
        {
            Server.Get<OnBuyNoAds>().RemoveListener(UpdateUI);
            InAppPurchaseAnalyticStruct iapAnalyticStruct = new InAppPurchaseAnalyticStruct();
            iapAnalyticStruct = iapAnalyticStruct.SetBaseIAP(UiHolderManager.Instance.GetCurrentPlacement(), IAPShowType.shop, TriggerType.click, "Null")
                .SetIAPClose(Time.time - AnalyticManager.Instance.timeOpenPopupIap);
            Server.Get<OnIAPCloseEventLog>().Dispatch(iapAnalyticStruct);
        }

        private void UpdateUI()
        {
            m_removeAdsPacks.gameObject.SetActive(!ShopManager.Instance.HasPurchasedNoAdsPack);
        }

        public void Init()
        {
            MyUlti.RemoveAllChilds(m_listBundlePackContainer);
            foreach(var pack in m_listBundlePacks.listShopPack)
            {
                var newPack = Instantiate(m_bundlePackPrefab, m_listBundlePackContainer);
                newPack.SetData(pack);
                newPack.OnPurchased = () =>
                {
                    m_goldBar.gameObject.SetActive(false);
                    var popupReceiveCoin = UIManager.Instance.ShowPopup<PopupReceiveCoin>(null);
                    var goldQuantity = pack.listReward.Find(x => x.type == ItemType.GOLD).quantity;
                    popupReceiveCoin.PlayCoinFX(m_goldBar.transform.position, Vector3.zero, goldQuantity, () =>
                    {
                        m_goldBar.gameObject.SetActive(true);
                    });
                };
            }
            MyUlti.RemoveAllChilds(m_listShopCoinPackContainer);
            foreach(var pack in m_listCoinPacks.listShopPack)
            {
                var newCoinPack = Instantiate(m_coinPackPrefab, m_listShopCoinPackContainer);
                newCoinPack.SetData(pack);
                newCoinPack.OnPurchased = () =>
                {
                    m_goldBar.gameObject.SetActive(false);
                    var popupReceiveCoin = UIManager.Instance.ShowPopup<PopupReceiveCoin>(null);
                    var goldQuantity = pack.listReward.Find(x => x.type == ItemType.GOLD).quantity;
                    popupReceiveCoin.PlayCoinFX(m_goldBar.transform.position, Vector3.zero, goldQuantity, () =>
                    {
                        m_goldBar.gameObject.SetActive(true);
                    });
                };
            }
            m_freeCoinPack.OnGotCoin = (val) => {
                m_goldBar.gameObject.SetActive(false);
                var popupReceiveCoin = UIManager.Instance.ShowPopup<PopupReceiveCoin>(null);
                popupReceiveCoin.PlayCoinFX(m_goldBar.transform.position, Vector3.zero, val, () =>
                {
                    m_goldBar.gameObject.SetActive(true);
                    m_goldBar.SetText(UserDataManager.Gold);
                });
            };
        }
    }
}
