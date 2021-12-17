using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class WallpapersMenu : MonoBehaviour
{
    public MainMenu mainMenu;
    public Transform wallpapersCanvas;
    public GameObject noEnoughCashPanel;

    public Animator cashTextAnimator;
    public Text cashText;

    [Header("Audio Stuff")]
    public AudioClip selectAudioClip;
    public AudioClip buyAudioClip;
    public AudioSource audioSource;

    [Header("Scrollbar")]
    public float multiplyingValue = 324.15f;
    public Scrollbar scrollbar;
    public Transform content;

    [Header("Wallpaper Stuff")]
    public int wallpapersPrice;

    int currentButtonViewedIndex;
    int lastSelectedWallpaper;
    int selectedWallpaperIndex;

    public List<ButtonProperties> buttonProperties = new List<ButtonProperties>();

    [System.Serializable]
    public class ButtonProperties
    {
        public int price;
        public RawImage wallpaperImage;
        public int wallpaperIndex;
        public bool isPurchased = false;
        public bool isSelected = false;

        public Text priceText;
        public Button selectionButton;
        public GameObject lockerImageGO;
    }

    void OnEnable()
    {
        isCoroutineStarted = false;
        UpdateCashText();

        ScrollToSelectedWallpaper();
        ScaleDownAroundSelectedButton();
    }

    void OnDisable()
    {
        DisableAnimators();
    }

    public void LoadWallpapersMenu() //Called By MainMenu Awake Function.
    {
        OpenSlotsInButtonPropList();
        MakeFirstWallpaperFree();
        //ChangeScrollbarNumberOfSteps();
        UpdateButtonProperties();
    }

    void ChangeScrollbarNumberOfSteps()
    {
        scrollbar.numberOfSteps = content.childCount;
    }

    void UpdateButtonProperties()
    {
        for (int i = 0; i < buttonProperties.Count; i++)
        {
            var buttonProp = buttonProperties[i];

            buttonProp.wallpaperIndex = i;

            buttonProp.wallpaperImage = content.GetChild(i).GetComponent<RawImage>();
            RawImage wallpaperCanvasImage = wallpapersCanvas.GetChild(i).GetComponent<RawImage>();
            buttonProp.wallpaperImage.texture = wallpaperCanvasImage.texture;
            buttonProp.wallpaperImage.uvRect = wallpaperCanvasImage.uvRect;

            buttonProp.price = wallpapersPrice * i;
            buttonProp.priceText = buttonProp.wallpaperImage.transform.GetChild(0).GetComponent<Text>();
            buttonProp.priceText.text = buttonProp.price.ToString();

            buttonProp.selectionButton = buttonProp.wallpaperImage.transform.GetChild(1).GetComponent<Button>();
            buttonProp.lockerImageGO = buttonProp.wallpaperImage.transform.GetChild(2).gameObject;
        }

        LoadPurchasedWallpaper();
    }

    public void ButtonClicked(int i)
    {
        selectedWallpaperIndex = i;

        var buttonProp = buttonProperties[i];

        if (buttonProp.isPurchased)
        {
            if (buttonProp.isSelected)
            {
                return;
            }
            else
            {
                SwitchBetweenPurchasedWallpapers(i);
                return;
            }
        }
        else
        {
            BuyWallpaper(i);
        }
    }

    void BuyWallpaper(int i)
    {
        i = selectedWallpaperIndex;
        var buttonProp = buttonProperties[i];

        if (mainMenu.rewardedCash >= buttonProp.price)
        {
            mainMenu.rewardedCash -= buttonProp.price;
            mainMenu.SaveRewardedCash();
            StartCoroutine(TextAnimation(cashTextAnimator, cashText, mainMenu.rewardedCash, 2f));
            //UpdateCashText();

            buttonProp.isPurchased = true;
            SwitchBetweenPurchasedWallpapers(i);

            SavePurchasedWallpaper();

            PlayAudioClip(buyAudioClip);
        }
        else
        {
            DoNotHaveEnoughCash();
        }
    }

    void SwitchBetweenPurchasedWallpapers(int currentSelectedItem)
    {
        for (int i = 0; i < buttonProperties.Count; i++)
        {
            var buttonProp = buttonProperties[i];

            if (i == currentSelectedItem)
            {
                buttonProp.isSelected = true;
                WallpaperButtonSelectedUI(true, i);
                WallpaperLockedUI(false, i);
                ApplyTheChanges(i);
                DataSaveManager.SaveInt("LSW", i); //shortcut for last selected Wallpaper.
                PlayAudioClip(selectAudioClip);
            }
            else
            {
                buttonProp.isSelected = false;
                WallpaperButtonSelectedUI(false, i);
            }
        }
    }

    void SavePurchasedWallpaper()
    {
        for (int i = 0; i < buttonProperties.Count; i++)
        {
            var buttonProp = buttonProperties[i];

            if (buttonProp.isPurchased && !DataSaveManager.IsDataExist("PW" + i)) //shortcut for purchased Wallpaper.
            {
                DataSaveManager.SaveBoolean("PW" + i, buttonProp.isPurchased);
            }
        }
    }

    void LoadPurchasedWallpaper()
    {
        for (int i = 0; i < buttonProperties.Count; i++)
        {
            var buttonProp = buttonProperties[i];

            if (DataSaveManager.IsDataExist("PW" + i))
            {
                buttonProp.isPurchased = DataSaveManager.LoadBoolean("PW" + i);

                if (buttonProp.isPurchased)
                {
                    WallpaperLockedUI(false, i);
                    WallpaperButtonSelectedUI(false, i);
                }
            }
            else
            {
                WallpaperLockedUI(true, i);
                if (buttonProp.isPurchased && buttonProp.isSelected)
                {
                    SwitchBetweenPurchasedWallpapers(i);
                }
            }

            if (DataSaveManager.IsDataExist("LSW"))
            {
                SwitchBetweenPurchasedWallpapers(DataSaveManager.LoadInt("LSW"));
            }
        }
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

    void WallpaperButtonSelectedUI(bool selected, int wallpaperIndex)
    {
        var buttonProp = buttonProperties[wallpaperIndex];

        if (selected)
        {
            ColorBlock buttonColor = buttonProp.selectionButton.colors;
            buttonColor.normalColor = Color.gray;
            buttonProp.selectionButton.colors = buttonColor;

            buttonProp.selectionButton.GetComponentInChildren<Text>().text = "Selected";

            buttonProp.selectionButton.interactable = false;
        }
        else
        {
            ColorBlock buttonColor = buttonProp.selectionButton.colors;
            buttonColor.normalColor = Color.white;
            buttonProp.selectionButton.colors = buttonColor;

            buttonProp.selectionButton.GetComponentInChildren<Text>().text = "Select";

            buttonProp.selectionButton.interactable = true;
        }
    }

    void WallpaperLockedUI(bool locked, int wallpaperIndex)
    {
        var buttonProp = buttonProperties[wallpaperIndex];

        if (locked)
        {
            buttonProp.priceText.gameObject.SetActive(true);

            buttonProp.lockerImageGO.SetActive(true);

            Color iconColor = buttonProp.wallpaperImage.color;
            iconColor.a = 0.5f;
            buttonProp.wallpaperImage.color = iconColor;
        }
        else
        {
            buttonProp.priceText.gameObject.SetActive(false);

            buttonProp.lockerImageGO.SetActive(false);

            Color iconColor = buttonProp.wallpaperImage.color;
            iconColor.a = 1f;
            buttonProp.wallpaperImage.color = iconColor;
        }
    }

    void ApplyTheChanges(int wallpaperIndex)
    {
        lastSelectedWallpaper = wallpaperIndex;

        foreach(Transform child in wallpapersCanvas)
        {
            child.gameObject.SetActive(false);
        }

        wallpapersCanvas.GetChild(wallpaperIndex).gameObject.SetActive(true);
    }

    void MakeFirstWallpaperFree()
    {
        var buttonProp = buttonProperties[0];

        buttonProp.isPurchased = true;
        buttonProp.isSelected = true;
    }

    void OpenSlotsInButtonPropList()
    {
        for (int i = 0; i < content.childCount; i++)
        {
            buttonProperties.Add(new ButtonProperties());
        }
    }

    void ScrollToSelectedWallpaper()
    {
        content.localPosition = new Vector2(-(lastSelectedWallpaper * multiplyingValue), content.localPosition.y);
    }

    void UpdateCashText()
    {
        cashText.text = mainMenu.rewardedCash.ToString();
    }

    void DisableAnimators()
    {
        cashTextAnimator.enabled = false;

        cashText.transform.localScale = Vector3.one;
    }

    bool isCoroutineStarted;

    public void ScaleDownAroundSelectedButton()
    {
        currentButtonViewedIndex = ((int)((-content.localPosition.x) / multiplyingValue));

        if(currentButtonViewedIndex <= (content.childCount - 1) && currentButtonViewedIndex > 0)
        {
            content.GetChild(currentButtonViewedIndex).transform.localScale = new Vector3(1f, 1f, 1f);
        }

        if ((currentButtonViewedIndex - 1) >= 0)
        {
            content.GetChild(currentButtonViewedIndex - 1).transform.localScale = new Vector3(0.85f, 0.85f, 1f);
        }

        if ((currentButtonViewedIndex + 1) < content.childCount)
        {
            content.GetChild(currentButtonViewedIndex + 1).transform.localScale = new Vector3(0.85f, 0.85f, 1f);
        }

        if (!isCoroutineStarted)
        {
            StartCoroutine(CheckIfScrollingIsStopped());
        }
    }

    IEnumerator CheckIfScrollingIsStopped()
    {
        isCoroutineStarted = true;
        float lastXPos;
        while (true)
        {
            lastXPos = content.localPosition.x;
            yield return new WaitForSeconds(0.1f);
            if(content.localPosition.x == lastXPos)
            {
                StartCoroutine(CenterTheButton());
                isCoroutineStarted = false;
                yield break;
            }

            yield return null;
        }
    }

    IEnumerator CenterTheButton()
    {
        float centerdButtonPosition = (-(currentButtonViewedIndex * multiplyingValue));
        float xPos;

        while (true)
        {
            centerdButtonPosition = (-(currentButtonViewedIndex * multiplyingValue));

            if (content.localPosition.x <= (centerdButtonPosition + 1f) && (centerdButtonPosition - 1f) <= content.localPosition.x)
            {
                yield break;
            }

            if (content.localPosition.x != centerdButtonPosition)
            {
                xPos = Mathf.Lerp(content.localPosition.x, centerdButtonPosition, 2f * Time.deltaTime);
                content.localPosition = new Vector2(xPos, content.localPosition.y);
            }

            yield return null;
        }
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

    void PlayAudioClip(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }
}
