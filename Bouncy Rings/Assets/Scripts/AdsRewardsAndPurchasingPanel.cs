using UnityEngine;
using UnityEngine.UI;

public class AdsRewardsAndPurchasingPanel : MonoBehaviour
{
    public Player player;
    public MainMenu mainMenu;

    public int cashRewardCount = 100;
    public int magnetsRewardCount = 2;

    public GameObject parentPanel;
    public GameObject purchasingCompletedPanel;
    public GameObject cashRewardImage;
    public GameObject adsRewardPanel;
    public Text purchasingCompletedMessageText;
    public Text cashRewardText;
    public Text magnetsRewardText;

    public void ApplyTheRewards()
    {
        mainMenu.rewardedCash += cashRewardCount;
        mainMenu.SaveRewardedCash();

        player.magnetCount += magnetsRewardCount;
        DataSaveManager.SaveInt("MC", player.magnetCount);

        UpdateRewardsUI();

        purchasingCompletedPanel.SetActive(false);
        adsRewardPanel.SetActive(true);
        parentPanel.SetActive(true);
    }

    void UpdateRewardsUI()
    {
        cashRewardText.text = cashRewardCount.ToString();
        magnetsRewardText.text = magnetsRewardCount.ToString();
    }

    public void ShowPurchasingCompletedMessage(string message)
    {
        purchasingCompletedMessageText.text = "Congratulations! You Have Purchased " + message;

        adsRewardPanel.SetActive(false);
        purchasingCompletedPanel.SetActive(true);
        cashRewardImage.SetActive(true);
        parentPanel.SetActive(true);
    }

    public void ShowPurchasingNoAdsMessage()
    {
        purchasingCompletedMessageText.text = "Congratulations! You Have Purchased No Ads";

        adsRewardPanel.SetActive(false);
        purchasingCompletedPanel.SetActive(true);
        cashRewardImage.SetActive(false);
        parentPanel.SetActive(true);
    }
}
