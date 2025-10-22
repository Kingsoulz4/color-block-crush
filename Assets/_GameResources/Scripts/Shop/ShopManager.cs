using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush
{
    public class ShopManager : SingletonMono<ShopManager>
    {
        [SerializeField] private ShopData m_listCoinPacks;
        [SerializeField] private ShopData m_listBundlePack;
        [SerializeField] private ShopData m_listRemoveAdsPacks;
        [SerializeField] private ShopData m_listDataShop;

        public bool HasPurchasedNoAdsPack
        {
            get => PlayerPrefs.GetInt("HasPurchasedNoAdsPack", 0) > 0;
            set => PlayerPrefs.SetInt("HasPurchasedNoAdsPack", value ? 1 : 0);
        }
        
        public bool HasPurchasedStarterPack
        {
            get => PlayerPrefs.GetInt("HasPurchasedStarterPack", 0) > 0;
            set => PlayerPrefs.SetInt("HasPurchasedStarterPack", value ? 1 : 0);
        }

        private List<string> listPurchasedPack;

        public List<string> ListPurchasedPacks
        {
            get
            {
                if(listPurchasedPack == null)
                {
                    listPurchasedPack = JsonConvert.DeserializeObject<List<string>>(PlayerPrefs.GetString("ListPurchasedPacks", "[]"));
                }
                return listPurchasedPack;
            }

            set
            {
                listPurchasedPack = value;
                PlayerPrefs.SetString("ListPurchasedPacks", JsonConvert.SerializeObject(listPurchasedPack));
            }
        }

        public void AddPurchasedPack(ShopPack packID)
        {
            if(!ListPurchasedPacks.Contains(packID.id))
            {
                ListPurchasedPacks.Add(packID.id);
                ListPurchasedPacks = ListPurchasedPacks;
            }
        }
         public void CheckReMoveAds() {

        }


        private void Awake()
        {
            Init();
        }

        void Init()
        {
            foreach(var item in m_listCoinPacks.listShopPack)
            {
                IAPManager.Instance.AddProduct(item.type,item.id, item.googleID, item.appleID, null);
            }
            
            foreach (var item in m_listBundlePack.listShopPack)
            {
                IAPManager.Instance.AddProduct(item.type,item.id, item.googleID, item.appleID, null);
            }
            foreach (var item in m_listDataShop.listShopPack)
            {
                IAPManager.Instance.AddProduct(item.type,item.id, item.googleID, item.appleID, null);
            }
            
            foreach (var item in m_listRemoveAdsPacks.listShopPack)
            {
                IAPManager.Instance.AddProduct(item.type,item.id, item.googleID, item.appleID, (Success) => {
                    if(Success)
                    Debug.Log("Remove ads");
                });
            }
        }
    } 
}