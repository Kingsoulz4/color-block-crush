using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ColorBlockCrush;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Models;
using UnityEngine.Purchasing.Security; // keep if you use validation later

public class UnityIAPManger : IAPHandlerBase {
    private StoreController _storeController;
    // Keep your callback dictionaries (internal ID -> callback)
    private Dictionary<string, UnityAction<bool>> productCallbackDict = new Dictionary<string, UnityAction<bool>>();
    private Dictionary<string, UnityAction<bool>> productCallbackRuntimeDict = new Dictionary<string, UnityAction<bool>>();

    // local struct to hold the product definitions you were building previously
    private class ProductMeta {
        public string internalId;
        public string googleStoreId;
        public string appleStoreId;
        public ProductType productType;
    }
    private readonly List<ProductMeta> _metaList = new List<ProductMeta>();
    private async void Start() {
        // Optionally call Init() from elsewhere instead of auto-start
        await InitializeAsync();
    }

    

    #region Initialization / fetch / events

    public bool IsInitialized() {
        return _storeController != null;
    }

    private async Task InitializeAsync() {
        if (_storeController != null)
            return;
        _storeController = UnityEngine.Purchasing.UnityIAPServices.StoreController();
        _storeController.OnProductsFetched += OnProductsFetched;
        _storeController.OnPurchasesFetched += OnPurchasesFetched;
        _storeController.OnPurchaseFailed += OnPurchaseFailed;
        _storeController.OnPurchasePending += OnPurchasePending;
        _storeController.OnPurchaseConfirmed += OnPurchaseConfirmed;

        try {
            await _storeController.Connect();
            Debug.Log("IAP v5 connected to store.");

            var productDefinitions = new List<ProductDefinition>();
            var catalogProvider = new CatalogProvider();
            foreach (var meta in _metaList) {
                var storeIds = new StoreSpecificIds();
                if (!string.IsNullOrEmpty(meta.googleStoreId))
                    storeIds.Add(meta.googleStoreId, GooglePlay.Name);
                if (!string.IsNullOrEmpty(meta.appleStoreId))
                    storeIds.Add(meta.appleStoreId, AppleAppStore.Name);

                //var pd = new ProductDefinition( );
                catalogProvider.AddProduct(meta.internalId, meta.productType, storeIds);
                //productDefinitions.Add(catalogProvider);
            }

            if (productDefinitions.Count > 0) {
                //_storeController.FetchProducts(productDefinitions);

            }


            catalogProvider.FetchProducts((pds) => {
                _storeController.FetchProducts(pds);
            });

        } catch (Exception ex) {
            Debug.LogError($"IAP Connect error: {ex.Message}");
        }


    }
    public override void AddProductType(ProductType type, string id, string idStoreGoogle, string idStoreApple, UnityAction<bool> callbackPurchase) {
        _metaList.Add(new ProductMeta {
            internalId = id,
            productType = type,
            googleStoreId = idStoreGoogle,
            appleStoreId = idStoreApple
        });

        if (!productCallbackDict.ContainsKey(id))
            productCallbackDict.Add(id, callbackPurchase);
        else
            productCallbackDict[id] = callbackPurchase;
    }
    public override bool CheckHasPurchased(string productId) {
        if (_storeController == null) {
            Debug.Log(" StoreController NUll");
            return false;
        }
        ReadOnlyObservableCollection<Order> orders = _storeController.GetPurchases();
        if (orders == null || orders.Count == 0) {
            Debug.Log("order Null");
            return false;
        }
        foreach (var order in orders) {
            if (order?.Info == null || order.Info.PurchasedProductInfo == null)
                continue;

            foreach (var info in order.Info.PurchasedProductInfo) {
                string purchasedId = info.productId;
                Debug.Log($"purchasedId purchase: {purchasedId}");

                if (purchasedId == productId) {
                    Debug.Log($"Product ID buy: {productId} {info.subscriptionInfo.GetExpireDate()}");
                    return true;
                }
            }
        }
        Debug.Log($" {productId}  Buy Fail.");
        return false;
    }

    private void OnPurchaseConfirmed(Order order) {
        var product = _storeController.GetProductById(order.Info.PurchasedProductInfo.First().productId);
        GrantRewardsForStoreId(product.definition.id);
    }

    private void OnProductsFetched(List<Product> products) {

        //Debug.Log($"OnProductsFetched: {products?.Count ?? 0} products.");
        //foreach (var p in products) {
        //    Debug.Log($"Product: {p.definition.id} | title = {p.metadata?.localizedTitle} | price = {p.metadata?.localizedPriceString}");
        //    //p.hasReceipt
        //}
        _storeController.FetchPurchases();
    }

    private void OnPurchasesFetched(Orders orders) {

        Debug.Log("OnPurchasesFetched: processing orders..." + orders.ToString());

        Debug.Log(" === IAP orders.ConfirmedOrders.Count." + orders.ConfirmedOrders.Count);
        if (orders.ConfirmedOrders.Count <= 0) {
            Debug.Log("orders Null...");
            return;
        }


        _storeController.ProcessPendingOrdersOnPurchasesFetched(true);

        foreach (var order in orders.PendingOrders) {
            if (order is PendingOrder pending) {
                Debug.Log($"Pending order found: {pending.Info.PurchasedProductInfo.First().productId} - You should handle UI for pending purchases.");
            }
        }
        //foreach (var order in orders.ConfirmedOrders) {
        //    if (order is ConfirmedOrder completed) {
        //        foreach (var po in completed.Info.PurchasedProductInfo) {
        //            GrantRewardsForStoreId(po.productId);
        //        }
        //    }
        //}

    }
    public override void BuyProductID(string internalProductId, UnityAction<bool> callback = null) {
        if (callback != null)
            productCallbackRuntimeDict[internalProductId] = callback;
        else if (productCallbackRuntimeDict.ContainsKey(internalProductId))
            productCallbackRuntimeDict.Remove(internalProductId);
        if (_storeController == null) {
            Debug.LogError("IAP not connected yet. Call Init() and wait for connection.");
            return;
        }
        var meta = _metaList.Find(m => m.internalId == internalProductId);
        if (meta == null) {
            Debug.LogError($"BuyProductID: no product meta for internal id {internalProductId}");
            return;
        }
        try {
            // purchase by internalId, StoreSpecificIds will map it
            _storeController.PurchaseProduct(internalProductId);
            Debug.Log($"Purchasing product: {internalProductId}");
        } catch (Exception ex) {
            Debug.LogError($"Purchase error: {ex.Message}");
        }
    }
    private void OnPurchasePending(PendingOrder pending) {
        Debug.Log($"OnPurchasePending: {pending.Info.TransactionID}");
        _storeController.ConfirmPurchase(pending);
    }
    
    private void OnPurchaseFailed(FailedOrder failed) {
        Debug.Log("Buy Fail:  "+ failed.ToString());
        var product = _storeController.GetProductById(failed.Info.PurchasedProductInfo.First().productId);
        if (productCallbackRuntimeDict.ContainsKey(product.definition.id))
            productCallbackRuntimeDict[product.definition.id].Invoke(false);
       //// var productId = failed.Info.PurchasedProductInfo.First().productId;
       // Debug.LogError($"OnPurchaseFailed: productId={productId} reason={failed.FailureReason}");

       // var meta = _metaList.Find(m => m.internalId == productId);
       // if (meta != null) {
       //     if (productCallbackDict.TryGetValue(meta.internalId, out var cb) && cb != null) cb.Invoke(false);
       //     if (productCallbackRuntimeDict.TryGetValue(meta.internalId, out cb) && cb != null) cb.Invoke(false);
       // }
    }
    
    public override float GetLocalizedPrice(string pPackageId)
    {
        try
        {
            if (IsInitialized())
            {
                var product = _storeController.GetProductById(pPackageId);
                if (product != null)
                    return (float)product.metadata.localizedPrice;
            }
               
            return 0;
        }
        catch (System.Exception)
        {
            return 0;
        }
    }
    
    public override string GetLocalizedPriceString(string pPackageId)
    {
        try
        {
            if (IsInitialized())
            {
                var product = _storeController.GetProductById(pPackageId);
                if (product != null)
                    return product.metadata.localizedPriceString;
            }
                   
            return "$0.00";
        }
        catch (System.Exception)
        {
            return "$0.00";
        }
    }
    #endregion
    #region Helpers: grant rewards
    private void GrantRewardsForStoreId(string storeId) {
        var meta = _metaList.Find(m => m.internalId == storeId);
        for (int i = 0; i < _metaList.Count; i++) {
            Debug.LogWarning($"IAP {_metaList[i].internalId}");
        }
        Debug.LogWarning($"iap:  {storeId} ==== ");
        if (meta == null) {
            Debug.LogWarning($"GrantRewards: no meta found for storeId {storeId}");
            return;
        }

        try {
            if (productCallbackDict.TryGetValue(meta.internalId, out var cb) && cb != null)
                cb.Invoke(true);

            if (productCallbackRuntimeDict.TryGetValue(meta.internalId, out cb) && cb != null) {
                cb.Invoke(true);
                productCallbackRuntimeDict.Remove(meta.internalId);
            }
            Debug.LogWarning($"====IAP storeId {storeId}");
            PushEventIAP_ForStoreId(storeId);
        } catch (KeyNotFoundException) { }
    }
    #endregion

    #region Stubs for old helpers
    public void PushEventIAP(PurchaseEventArgs e) {
        Debug.Log($"PushEventIAP (deprecated stub) - received storeId from legacy args.");
    }

    private void PushEventIAP_ForStoreId(string storeId) {
        Debug.Log($"PushEventIAP_ForStoreId: {storeId}");
    }
    #endregion

    #region Validate
    //private bool ValidatePurchase(Product product) {
    //    bool validPurchase = true;
    //    var validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
    //        AppleTangle.Data(), Application.identifier);
    //    try {
    //        var result = validator.Validate(product.receipt);
    //        foreach (IPurchaseReceipt productReceipt in result) {
    //            if (productReceipt.purchaseDate >= DateTime.UtcNow.AddMinutes(-5f)) {
    //               // LogAFPurchase(product);
    //            }
    //        }
    //    } catch (IAPSecurityException) {
    //        validPurchase = false;
    //    }
    //    return validPurchase;
    //}
    #endregion
    #region Sub
    public override bool IsSubscribed(string productId) {
        if (_storeController == null) {
            Debug.Log(" StoreController NUll");
            return false;
        }
        ReadOnlyObservableCollection<Order> orders = _storeController.GetPurchases();
        if (orders == null || orders.Count == 0) {
            Debug.Log("order Null");
            return false;
        }
        Debug.Log("IAP orders: "+ orders.Count);
        foreach (var order in orders) {
            if (order?.Info == null || order.Info.PurchasedProductInfo == null)
                continue;
            foreach (var info in order.Info.PurchasedProductInfo) {
                string purchasedId = info.productId;
                Debug.Log($"purchasedId purchase: {purchasedId}");

                var product = _storeController.GetProductById(purchasedId);

                if (product.definition.id == productId) {
                  //  Debug.Log($"Product ID buy: {productId} {info.subscriptionInfo.GetExpireDate()}");
                    Debug.Log($"Product ID buy: {productId} {info.subscriptionInfo.GetExpireDate()}");
                    if (info.subscriptionInfo.GetExpireDate() > DateTime.Now) {
                        return true;
                    }
                }
            }
        }
        Debug.Log($" {productId}  Buy Fail.");
        return false;
    }
    public DateTime GetDateTimeSub(string productId) {
     
        ReadOnlyObservableCollection<Order> orders = _storeController.GetPurchases();
        if (orders == null || orders.Count == 0) {
            Debug.Log("order Null");
            return DateTime.Now;
        }
        foreach (var order in orders) {
            if (order?.Info == null || order.Info.PurchasedProductInfo == null)
                continue;
            foreach (var info in order.Info.PurchasedProductInfo) {
                string purchasedId = info.productId;
                Debug.Log($"purchasedId purchase: {purchasedId}");
                if (purchasedId == productId) {
                    Debug.Log($"Product ID buy: {productId} {info.subscriptionInfo.GetExpireDate()}");
                    if (info.subscriptionInfo.GetExpireDate() > DateTime.Now) {
                        return info.subscriptionInfo.GetExpireDate();
                    }
                }
            }
        }
        return DateTime.Now;

    } 
    #endregion

}