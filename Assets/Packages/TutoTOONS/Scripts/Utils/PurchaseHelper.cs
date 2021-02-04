#if GEN_HAS_UNITY_IAP
using UnityEngine.Purchasing;
#endif

using System.Collections.Generic;

namespace TutoTOONS
{
    public enum ProductTypeIAP
    {
        NoAds,
        UnlockAll,
        Subscription
    }

    public class PurchaseHelper
    {
        #if GEN_HAS_UNITY_IAP
        public static IStoreController store_controller {get;private set;}
        public static bool store_initialized { get; private set; }
        public static string no_ads_sku { get; private set; }
        public static string unlock_all_sku { get; private set; }
        #endif

        #if GEN_HAS_UNITY_IAP
        public static void SetInitialized()
        {

        }
        public static void SetIAPController(IStoreController _store_controller)
        {
            store_controller = _store_controller;
        }
        public static void PurchasedIAP(Product _product, Dictionary<string, object> _attributes = null)
        {

        }
        public static void RestoredIAP(Product _product, Dictionary<string, object> _attributes = null)
        {

        }

        public static bool ProductIsPurchased(string _id)
        {
            if (string.IsNullOrEmpty(_id))
            {
                return false;
            }

            if (store_controller != null)
            {
                Product _product = store_controller.products.WithID(_id);
                if (_product != null)
                {
                    return _product.hasReceipt;
                }
            }

            return false;
        }
        #endif
    }
}
