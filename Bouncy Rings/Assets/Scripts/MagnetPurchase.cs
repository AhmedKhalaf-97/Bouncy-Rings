using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;


public class MagnetPurchase : MonoBehaviour
{
    public MainMenu mainMenu;
    public Player player;

    public Text cashText;
    public Text magnetCountText;

    public Animator cashTextAnimator;
    public Animator magnetCountTextAnimator;

    public GameObject noEnoughCashPanel;

    public List<ButtonProperties> buttonProperties = new List<ButtonProperties>();

    [System.Serializable]
    public class ButtonProperties
    {
        public Button myButton;
        public int purchasedMagnetCountAmount;
        public Sprite magnetImage;
        public int requiredCash;
    }

    void Awake()
    {
        UpdateButtonPropertiesUI();
    }

    void OnEnable()
    {
        UpdateCashAndMagnetUI();
    }

    void OnDisable()
    {
        DisableAnimators();
    }

    void UpdateButtonPropertiesUI()
    {
        for (int i = 0; i < buttonProperties.Count; i++)
        {
            var buttonProp = buttonProperties[i];
            buttonProp.myButton.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = buttonProp.magnetImage;
            buttonProp.myButton.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "x " + buttonProp.purchasedMagnetCountAmount.ToString();
            buttonProp.myButton.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = buttonProp.requiredCash.ToString();
        }
    }

    public void BuyMagnets(int i)
    {
        var buttonProp = buttonProperties[i];

        if(buttonProp.requiredCash <= mainMenu.rewardedCash)
        {
            mainMenu.rewardedCash -= buttonProp.requiredCash;
            mainMenu.SaveRewardedCash();
            StartCoroutine(TextAnimation(cashTextAnimator, cashText, mainMenu.rewardedCash, 2f));

            player.magnetCount += buttonProp.purchasedMagnetCountAmount;
            DataSaveManager.SaveInt("MC", player.magnetCount);
            StartCoroutine(TextAnimation(magnetCountTextAnimator, magnetCountText, player.magnetCount, 2f));

            //UpdateCashAndMagnetUI();
        }
        else
        {
            DoNotHaveEnoughCash();
        }
    }

    void UpdateCashAndMagnetUI()
    {
        cashText.text = mainMenu.rewardedCash.ToString();
        magnetCountText.text = player.magnetCount.ToString();
    }

    void DisableAnimators()
    {
        cashTextAnimator.enabled = false;
        magnetCountTextAnimator.enabled = false;

        cashText.transform.localScale = Vector3.one;
        magnetCountTextAnimator.transform.localScale = Vector3.one;
    }

    void DoNotHaveEnoughCash()
    {
        StartCoroutine(NoEnoughCashPanel());
    }

    IEnumerator NoEnoughCashPanel()
    {
        noEnoughCashPanel.SetActive(true);
        yield return new WaitForSeconds(2f);
        noEnoughCashPanel.SetActive(false);
    }


    IEnumerator TextAnimation(Animator textAnimator, Text textToAnimate, int countToReach, float duration)
    {
        float lerpSpeed = 0;
        int currnetCount = int.Parse(Regex.Replace(textToAnimate.text, @"[^\d]", ""));

        if (countToReach != currnetCount)
        {
            if (!textAnimator.enabled)
            {
                textAnimator.enabled = true;
            }

            while (currnetCount != countToReach)
            {
                lerpSpeed += Time.deltaTime / duration;
                currnetCount = (int)Mathf.Lerp(currnetCount, countToReach, lerpSpeed);
                textToAnimate.text = currnetCount.ToString();

                if (currnetCount == countToReach)
                {
                    lerpSpeed = 0;

                    textAnimator.enabled = false;
                    textToAnimate.transform.localScale = Vector3.one;
                    yield break;
                }
                yield return null;
            }
        }
    }
}