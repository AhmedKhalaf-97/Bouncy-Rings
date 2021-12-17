using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class DailyReward : MonoBehaviour
{
    int dailyRewardProgress;

    DateTime lastDateTime;
    DateTime todayDateTime;
    TimeSpan interval;

    public MainMenu mainMenu;
    public GameObject dailyRewardPanel;

    public List<Rewards> rewards;

    [System.Serializable]
    public class Rewards
    {
        public int rewardValue;
        public GameObject rewardPanel;

        [HideInInspector]
        public Text rewardValueText;
        [HideInInspector]
        public GameObject okImage;
        [HideInInspector]
        public Button takeItButton;
    }

    void Start()
    {
        dailyRewardProgress = DataSaveManager.LoadInt("DRP");

        lastDateTime = DataSaveManager.LoadDateTime("DTN");
        todayDateTime = DateTime.Now.Date;
        interval = todayDateTime - lastDateTime;

        IsEligibleForReward();

        SaveDateTime();

        UpdateRewardsListValueAndUI();
    }

    void IsEligibleForReward()
    {
        if(todayDateTime > lastDateTime)
        {
            Invoke("SetDailyRewardPanelActive", 0.6f);

            if (interval.Days > 1)
            {
                dailyRewardProgress = 0; //Reset progress.
                DataSaveManager.SaveInt("DRP", dailyRewardProgress);
            }
        }
        else
        {
            dailyRewardPanel.SetActive(false);
        }
    }

    public void TakeTheReward(int i)
    {
        mainMenu.rewardedCash += rewards[i].rewardValue;
        mainMenu.SaveRewardedCash();

        dailyRewardProgress++;

        if (dailyRewardProgress >= rewards.Count) //Reset after all rewards has been taken.
        {
            dailyRewardProgress = 0;
        }

        rewards[i].okImage.SetActive(true);
        rewards[i].takeItButton.gameObject.SetActive(false);

        DataSaveManager.SaveInt("DRP", dailyRewardProgress);
    }

    void SaveDateTime()
    {
        DataSaveManager.SaveDateTime("DTN", DateTime.Now.Date);
    }

    void SetDailyRewardPanelActive() //Called in IsEligibleForReward function.
    {
        dailyRewardPanel.SetActive(true);
    }

    void UpdateRewardsListValueAndUI()
    {
        for (int i = 0; i < rewards.Count; i++)
        {
            var reward = rewards[i];
            Transform rewardPanel = reward.rewardPanel.transform;

            reward.rewardValueText = rewardPanel.GetChild(3).GetChild(1).GetComponent<Text>();
            reward.okImage = rewardPanel.GetChild(1).gameObject;
            reward.takeItButton = rewardPanel.GetChild(2).GetComponent<Button>();

            //Update UI.
            reward.rewardValueText.text = reward.rewardValue.ToString();

            if(i == dailyRewardProgress)
            {
                reward.okImage.SetActive(false);
                reward.takeItButton.gameObject.SetActive(true);
                reward.takeItButton.interactable = true;
            }
            else if(i > dailyRewardProgress)
            {
                reward.okImage.SetActive(false);
                reward.takeItButton.gameObject.SetActive(true);
                reward.takeItButton.interactable = false;
            }

            if(i < dailyRewardProgress)
            {
                reward.okImage.SetActive(true);
                reward.takeItButton.gameObject.SetActive(false);
            }
        }
    }
}