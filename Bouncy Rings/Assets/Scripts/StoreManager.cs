using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
    public MainMenu mainMenu;

    public Purchaser purchaser;
    public AdsRewardsAndPurchasingPanel adsRewardsAndPurchasingPanel;
    public UnityAds unityAds;
    public Button noAdsButton;
    public Text noAdsPriceText;

    public List<StoreButtonProperties> storeButtonProperties = new List<StoreButtonProperties>();

    [System.Serializable]
    public class StoreButtonProperties
    {
        public Button myButton;
        public int purchasedCashAmount;
        public Sprite buttonImage;
        public float price;
        public string productID;
    }

    void Awake()
    {
        DisableNoAdsButton();

        UpdateStoreButtonPropertiesUI();

        InvokeRepeating("UpdatePricesFromStoreController", 1f, 5f);
    }

    void UpdateStoreButtonPropertiesUI()
    {
        for (int i = 0; i < storeButtonProperties.Count; i++)
        {
            var buttonProp = storeButtonProperties[i];
            buttonProp.myButton.transform.GetChild(0).GetComponent<Image>().sprite = buttonProp.buttonImage;
            buttonProp.myButton.transform.GetChild(1).GetComponent<Text>().text = string.Format("{0:0,0}", buttonProp.purchasedCashAmount);
            //buttonProp.myButton.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = buttonProp.price.ToString() + " USD";
        }
    }

    public void BuyItem(int i)
    {
        var buttonProp = storeButtonProperties[i];

        purchaser.BuyConsumable(buttonProp.productID);
    }

    public void ItemPurchased(string productID)
    {
        for (int i = 0; i < storeButtonProperties.Count; i++)
        {
            var buttonProp = storeButtonProperties[i];

            if (buttonProp.productID == productID)
            {
                mainMenu.rewardedCash += buttonProp.purchasedCashAmount;
                mainMenu.SaveRewardedCash();

                adsRewardsAndPurchasingPanel.ShowPurchasingCompletedMessage(string.Format("{0:0,0}", buttonProp.purchasedCashAmount));
            }
        }
    }

    void UpdatePricesFromStoreController()
    {
        for (int i = 0; i < storeButtonProperties.Count; i++)
        {
            var buttonProp = storeButtonProperties[i];

            if (purchaser.IsInitialized())
            {
                buttonProp.myButton.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = purchaser.m_StoreController.products.WithID(buttonProp.productID).metadata.localizedPrice
                + " " + purchaser.m_StoreController.products.WithID(buttonProp.productID).metadata.isoCurrencyCode;
            }
        }

        if (purchaser.IsInitialized())
        {
            noAdsPriceText.text = purchaser.m_StoreController.products.WithID("com.bantergames.bouncyrings.noads").metadata.localizedPrice
                  + " " + purchaser.m_StoreController.products.WithID("com.bantergames.bouncyrings.noads").metadata.isoCurrencyCode;
        }

        if (purchaser.m_StoreController.products.WithID(storeButtonProperties[0].productID).metadata.localizedPrice != 0)
        {
            CancelInvoke();
        }
    }

    public void DisableNoAdsButton()
    {
        if (unityAds.isAdsDisabled)
        {
            noAdsButton.interactable = false;
        }
    }
}
