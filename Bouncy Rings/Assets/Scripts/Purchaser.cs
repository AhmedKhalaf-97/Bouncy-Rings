using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;


public class Purchaser : MonoBehaviour, IStoreListener
{
    public IStoreController m_StoreController;          
    private static IExtensionProvider m_StoreExtensionProvider;

    public static string[] kProductsIDsConsumable =
        { "com.bantergames.bouncyrings.5kcash",
        "com.bantergames.bouncyrings.12kcash",
        "com.bantergames.bouncyrings.16kcash",
        "com.bantergames.bouncyrings.25kcash"};

    public static string kProductIDNonConsumable = "com.bantergames.bouncyrings.noads";

    public StoreManager storeManager;
    UnityAds unityAds;
    AdsRewardsAndPurchasingPanel adsRewardsAndPurchasingPanel;

    void Start()
    {
        unityAds = GetComponent<UnityAds>();
        adsRewardsAndPurchasingPanel = GetComponent<AdsRewardsAndPurchasingPanel>();

        if (m_StoreController == null)
        {
            InitializePurchasing();
        }
    }

    public void InitializePurchasing()
    {
        if (IsInitialized())
        {
            return;
        }

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        foreach(string productID in kProductsIDsConsumable)
        {
            builder.AddProduct(productID, ProductType.Consumable);
        }

        builder.AddProduct(kProductIDNonConsumable, ProductType.NonConsumable);

        UnityPurchasing.Initialize(this, builder);
    }


    public bool IsInitialized()
    {
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }


    public void BuyConsumable(string productID)
    {
        BuyProductID(productID);
    }


    public void BuyNonConsumable()
    {
        BuyProductID(kProductIDNonConsumable);
    }


    void BuyProductID(string productId)
    {
        if (!NetworkAvailability.instance.IsConnected())
        {
            return;
        }

        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(productId);

            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                m_StoreController.InitiatePurchase(product);
            }
            else
            {
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        else
        {
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }

    public void RestorePurchases()
    {
        if (!NetworkAvailability.instance.IsConnected())
        {
            return;
        }

        if (!IsInitialized())
        {
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            Debug.Log("RestorePurchases started ...");

            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();

            apple.RestoreTransactions((result) => {
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        else
        {
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("OnInitialized: PASS");

        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        for (int i = 0; i < kProductsIDsConsumable.Length; i++)
        {
            if (String.Equals(args.purchasedProduct.definition.id, kProductsIDsConsumable[i], StringComparison.Ordinal))
            {
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                storeManager.ItemPurchased(kProductsIDsConsumable[i]);
            }
        }

        if (String.Equals(args.purchasedProduct.definition.id, kProductIDNonConsumable, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            if (!unityAds.isAdsDisabled)
            {
                unityAds.isAdsDisabled = true;
                DataSaveManager.SaveBoolean("NA", true);

                storeManager.DisableNoAdsButton();

                adsRewardsAndPurchasingPanel.ShowPurchasingNoAdsMessage();
            }
        }

        return PurchaseProcessingResult.Complete;
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }
}