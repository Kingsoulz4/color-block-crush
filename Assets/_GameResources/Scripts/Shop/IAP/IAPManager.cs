using AYellowpaper.SerializedCollections;
using ColorBlockCrush;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Models;

public enum IAPProvider
{
    UNITY_IAP,
    QON_IAP
}

public class IAPManager : SingletonDontDestroyMono<IAPManager>, IHandleIAP
{
    [SerializeField] private SerializedDictionary<IAPProvider, IAPHandlerBase> listIAPProvider;

    private IHandleIAP IAPHandler
    { 
        get
        {
            return listIAPProvider[IAPProvider.UNITY_IAP];
        } 
    }

    protected override void Awake()
    {
        base.Awake();
    }

    public void AddProductType(ProductType type, string id, string idStoreGoogle, string idStoreApple, UnityAction<bool> callbackPurchase)
    {
        IAPHandler.AddProductType(type, id, idStoreGoogle, idStoreApple, callbackPurchase);
    }

    public void BuyProductID(string internalProductId, UnityAction<bool> callback = null) {
        #if UNITY_EDITOR
                callback?.Invoke(true);
                return;
        #endif
        IAPHandler.BuyProductID(internalProductId, callback);
      //  Debug.Log("buy:" + CheckHasPurchased(internalProductId));

    }

    public void Init()
    {
        IAPHandler.Init();
    }

    public void RestorePurchases()
    {
        IAPHandler.RestorePurchases();
    }

    //public void AddProductConsume(string id, string idStoreGoogle, string idStoreApple, UnityAction<bool> callbackPurchase)
    //{
    //    IAPHandler.AddProductConsume(id, idStoreGoogle, idStoreApple, callbackPurchase);
    //}

    //public void AddProductNonConsume(string id, string idStoreGoogle, string idStoreApple, UnityAction<bool> callbackPurchase)
    //{
    //    IAPHandler.AddProductNonConsume(id, idStoreGoogle, idStoreApple, callbackPurchase);
    //}

    //public void AddProductSubscription(string id, string idStoreGoogle, string idStoreApple, UnityAction<bool> callbackPurchase)
    //{
    //    IAPHandler.AddProductSubscription(id, idStoreGoogle, idStoreApple, callbackPurchase);
    //}

    public void AddProduct(ProductType type, string id, string idStoreGoogle, string idStoreApple, UnityAction<bool> callbackPurchase) {
        IAPHandler.AddProductType(type, id, idStoreGoogle, idStoreApple, callbackPurchase);
    }


    public float GetLocalizedPrice(string pPackageId)
    {
        return IAPHandler.GetLocalizedPrice(pPackageId);
    }

    public string GetLocalizedPriceString(string pPackageId)
    {
        return IAPHandler.GetLocalizedPriceString(pPackageId);
    }

    public bool CheckHasPurchased(string internalProductId) {
      return  IAPHandler.CheckHasPurchased(internalProductId);
    }
    public void Restore() {
        IAPHandler.RestorePurchases();
    }
    public bool IsSubscribed(string productId) {
        return IAPHandler.IsSubscribed(productId);
    }
}
