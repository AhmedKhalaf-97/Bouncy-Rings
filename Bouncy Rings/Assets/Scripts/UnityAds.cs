using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Monetization;
using UnityEngine.UI;

public class UnityAds : MonoBehaviour
{
    public bool isAdsDisabled;

    string gameId = "2945774";
    bool testMode = false;

    public string videoPlacementId = "video";
    public string rewardedVideoPlacementId = "rewardedVideo";

    public GameObject mainMenuCanvas;
    MainMenu mainMenu;
    AdsRewardsAndPurchasingPanel adsRewardsAndPurchasingPanel;

    public GameObject noAdsButton;

    void OnEnable()
    {
        mainMenu = mainMenuCanvas.GetComponent<MainMenu>();
        adsRewardsAndPurchasingPanel = GetComponent<AdsRewardsAndPurchasingPanel>();

        if (DataSaveManager.IsDataExist("NA"))
        {
            isAdsDisabled = DataSaveManager.LoadBoolean("NA");

            if (isAdsDisabled)
            {
                noAdsButton.SetActive(true);
            }
        }
    }

    void Start()
    {
        Monetization.Initialize(gameId, testMode);
    }

    public void ShowVideoAd()
    {
        if (!isAdsDisabled)
        {
            if (!Monetization.isInitialized)
            {
                Monetization.Initialize(gameId, testMode);
            }

            StartCoroutine(ShowAdWhenReady());
        }
    }

    private IEnumerator ShowAdWhenReady()
    {
        while (!Monetization.IsReady(videoPlacementId))
        {
            yield return new WaitForSeconds(0.25f);
        }

        ShowAdPlacementContent ad = null;
        ad = Monetization.GetPlacementContent(videoPlacementId) as ShowAdPlacementContent;

        if (ad != null)
        {
            ad.Show();
            mainMenu.gameSessions = 0;
        }
    }

    public void ShowRewardedVideoAd()
    {
        if (!NetworkAvailability.instance.IsConnected())
        {
            return;
        }
        else
        {
            if (!Monetization.isInitialized)
            {
                Monetization.Initialize(gameId, testMode);
            }
        }

        StartCoroutine(WaitForAd());
    }

    IEnumerator WaitForAd()
    {
        while (!Monetization.IsReady(rewardedVideoPlacementId))
        {
            yield return null;
        }

        ShowAdPlacementContent ad = null;
        ad = Monetization.GetPlacementContent(rewardedVideoPlacementId) as ShowAdPlacementContent;

        if (ad != null)
        {
            mainMenuCanvas.SetActive(false);

            ad.Show(AdFinished);
        }
    }

    void AdFinished(ShowResult result)
    {
        if (result == ShowResult.Finished)
        {
            StartCoroutine(ApplyTheRewards());
        }
    }

    IEnumerator ApplyTheRewards()
    {
        adsRewardsAndPurchasingPanel.ApplyTheRewards();

        yield return new WaitForSeconds(0.25f);

        mainMenuCanvas.SetActive(true);
    }
}
