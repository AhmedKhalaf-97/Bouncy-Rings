using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public bool isChallengeRunning;

    public Player player;
    public ChallengeLevels challengeLevels;
    public PlayModes playModes;
    public FloatingObjectSpawner floatingObjectSpawner;
    public GameObject hudCanvas;
    public GameObject pausePanel;
    public Text numOfObjects;
    public Animator mainMenuAnimator;

    public WallpapersMenu wallpapersMenu;
    public ConesCollisionDetection[] conesCollisionDetections;
    public int numOfInstantiedRings;

    public UnityAds unityAds;
    public int gameSessions;

    public GameObject specificModeHint;

    public GameObject firstTimePlayingHint;

    bool isRatePanelPopedUpToday;
    public GameObject rateTheGamePanel;

    [Header("Audio Stuff")]
    public AudioClip pauseSceneAC;
    public AudioClip resumeSceneAC;
    public AudioSource hudCanvasAudioSource;
    public GameObject outsideAudioSource;

    [Header("Score System")]
    public int latestScore;
    public Text hudScoreText;
    public Animator hudScoreTextAnimator;
    public Text mainMenuScoreText;
    public Text mainMenuBestScoreText;

    [Header("Level Manager")]
    public int currentLevelNum;
    public int requiredScore;
    public int cumulativeScore;
    public Text currentLevelNumText;
    public Slider levelSlider;

    [Header("Cash Reward")]
    public int playTime;
    public int cashFromScoredFloatingObjs;
    public int cashFromPlayingTime;
    public int totalCash;
    public int rewardedCash;
    public GameObject cashRewardPanel;
    public Text playingTimeRewardPanelText;
    public Text scoredObjsRewardPanelText;
    public Text cashFromScoredFloatingObjsText;
    public Text cashFromPlayingTimeText;
    public Text totalCashText;
    public Text rewardedCashText;
    public int floatingObjMultiplingValue = 10;
    public int playingTimeThreshold = 30;
    public int playingTimeCashThreshold = 100;

    void Start()
    {
        if (!Application.isMobilePlatform)
        {
            Screen.SetResolution(450, 1280, true);
        }
    }

    void Awake()
    {
        player.LoadSettings();
        player.LoadMagnetCount();

        latestScore = 0;
        hudScoreText.text = string.Format("{0:0,0}", latestScore);

        SetLevelNumberAndCumulativeScore();
        UpdateMainMenuScoreRelatedTexts();
        SetHighScore();

        rewardedCash = DataSaveManager.LoadInt("RC");
        rewardedCashText.text = rewardedCash.ToString();

        LoadGame();

        if (!DataSaveManager.LoadBoolean("FTPH"))
        {
            Invoke("ShowFirstTimePlayingHint", 0.6f);
        }
    }

    void OnEnable()
    {
        outsideAudioSource.SetActive(true);
    }

    public void StartGame(bool isChallenge)
    {
        StartCoroutine(StartGameCoroutine(isChallenge));
    }

    IEnumerator StartGameCoroutine(bool isChallenge)
    {
        if (isChallenge)
        {
            isChallengeRunning = true;
            challengeLevels.SetChallengeLevel();
        }

        playModes.SetTheScene();

        playModes.SetPlayerTimer();

        latestScore = 0;
        hudScoreText.text = string.Format("{0:0,0}", latestScore);

        player.gameObject.SetActive(true);
        numOfInstantiedRings = 0;
        player.scoredFloatingObjects = 0;
        player.ScoredObjectsCountText.text = player.scoredFloatingObjects.ToString();
        numOfObjects.text = floatingObjectSpawner.numberOfObjects.ToString();
        floatingObjectSpawner.gameObject.SetActive(true);
        player.CalculatePlayingTime(true);
        hudCanvas.SetActive(true);

        SetUIBasedOnPlayMode();
        foreach (ConesCollisionDetection cone in conesCollisionDetections)
        {
            cone.CheckIfIsSpecificCone();
        }

        player.ResetMagnet();

        mainMenuAnimator.SetTrigger("Close");
        yield return new WaitForSeconds(mainMenuAnimator.runtimeAnimatorController.animationClips[1].length);

        if (hudCanvas.activeInHierarchy)
        {
            gameObject.SetActive(false);
        }

        player.EnablePlayingInstructions();
    }

    public void EndGame()
    {
        if (Time.timeScale == 0)
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1;
        }

        UpdateMainMenuScoreRelatedTexts();
        SetHighScore();
        SetCumulativeScore();

        player.CalculatePlayingTime(false);
        player.gameObject.SetActive(false);
        floatingObjectSpawner.gameObject.SetActive(false);
        hudCanvas.SetActive(false);
        mainMenuAnimator.SetTrigger("Open");
        gameObject.SetActive(true);
        DestroyAllFloatingObjects();


        if (playModes.isEndlessGame && !playModes.isTimeTrialGame)
        {
            CalculateCashReward();
        }

        if (isChallengeRunning)
        {
            isChallengeRunning = false;
            challengeLevels.CheckIfChallengeCompleted();
        }

        if (!unityAds.isAdsDisabled)
        {
            ShowAdIfPossible();
        }

        CheckIfRatePanelShoudPopUp();
    }

    void DestroyAllFloatingObjects()
    {
        GameObject[] floatingObjects = GameObject.FindGameObjectsWithTag("Floating");
        foreach (GameObject go in floatingObjects)
        {
            Destroy(go);
        }
    }

    public void AddScore(int scoreToBeAdded, float timeIncreasingValue)
    {
        latestScore += scoreToBeAdded;
        hudScoreText.text = string.Format("{0:0,0}", latestScore);
        hudScoreTextAnimator.SetTrigger("AnimText");

        player.scoredFloatingObjects++;
        player.ScoredObjectsCountText.text = player.scoredFloatingObjects.ToString();
        player.playingTime += timeIncreasingValue;

        player.PlayScoreAudioClip();

        CheckIfGameCompleted();
    }

    void SetHighScore()
    {
        if (latestScore > DataSaveManager.LoadInt("HS")) //stand for HighScore.
        {
            DataSaveManager.SaveInt("HS", latestScore);
            mainMenuBestScoreText.text = string.Format("{0:0,0}", latestScore);
        }
        else
        {
            mainMenuBestScoreText.text = string.Format("{0:0,0}", DataSaveManager.LoadInt("HS"));
        }
    }

    void SetCumulativeScore()
    {
        cumulativeScore += latestScore;

        if (cumulativeScore >= requiredScore)
        {
            cumulativeScore -= requiredScore;
            ShiftToNextLevel();
        }

        DataSaveManager.SaveInt("CS", cumulativeScore); //stand for CumulativeScore.

        UpdateLevelInfoUI();
    }

    void SetLevelNumberAndCumulativeScore()
    {
        if (DataSaveManager.IsDataExist("LN"))
        {
            currentLevelNum = DataSaveManager.LoadInt("LN"); //stand for LevelNumber.
        }
        else
        {
            currentLevelNum = 1;
            DataSaveManager.SaveInt("LN", currentLevelNum);
        }

        requiredScore = currentLevelNum * 1000;

        cumulativeScore = DataSaveManager.LoadInt("CS");

        UpdateLevelInfoUI();
    }

    void ShiftToNextLevel()
    {
        currentLevelNum++;
        requiredScore = currentLevelNum * 1000;
        DataSaveManager.SaveInt("LN", currentLevelNum);

        UpdateLevelInfoUI();
    }

    void UpdateLevelInfoUI()
    {
        currentLevelNumText.text = currentLevelNum.ToString();

        Invoke("LevelSlider", 0.1f);
    }

    void LevelSlider()
    {
        UpdateLevelSlider(0, requiredScore, cumulativeScore, 0);
    }

    void UpdateLevelSlider(float minValue, float maxValue, float wantedValue, float currentValue)
    {
        StartCoroutine(UpdateLevelSliderCoroutine(minValue, maxValue, wantedValue, currentValue));
    }

    float duration = 2f;
    float lerpSpeed = 0;
    IEnumerator UpdateLevelSliderCoroutine(float minValue, float maxValue, float wantedValue, float currentValue)
    {
        lerpSpeed = 0;

        levelSlider.minValue = minValue;
        levelSlider.maxValue = maxValue;
        levelSlider.value = currentValue;

        while (true)
        {
            lerpSpeed += Time.deltaTime / duration;
            levelSlider.value = Mathf.Lerp(currentValue, wantedValue, lerpSpeed);

            if (wantedValue == levelSlider.value)
            {
                lerpSpeed = 0;
                yield break;
            }
            yield return null;
        }
    }

    void UpdateMainMenuScoreRelatedTexts()
    {
        mainMenuScoreText.text = string.Format("{0:0,0}", latestScore);
    }

    public void PauseScene(bool condition)
    {
        if (condition)
        {
            Time.timeScale = 0;
            pausePanel.SetActive(true);

            hudCanvasAudioSource.clip = pauseSceneAC;
            hudCanvasAudioSource.Play();
        }
        else
        {
            Time.timeScale = 1;
            pausePanel.SetActive(false);

            hudCanvasAudioSource.clip = resumeSceneAC;
            hudCanvasAudioSource.Play();
        }
    }

    public void ReloadGame()
    {
        if (Time.timeScale == 0)
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1;
        }

        DestroyAllFloatingObjects();

        latestScore = 0;
        hudScoreText.text = string.Format("{0:0,0}", latestScore);

        floatingObjectSpawner.GetComponent<FloatingObjectSpawner>().InstantiateObjects();

        player.scoredFloatingObjects = 0;
        numOfInstantiedRings = 0;
        player.ScoredObjectsCountText.text = player.scoredFloatingObjects.ToString();
        numOfObjects.text = floatingObjectSpawner.numberOfObjects.ToString();
        player.CalculatePlayingTime(true);

        player.ResetMagnet();

        challengeLevels.magnetCountBeforeChallenge = player.magnetCount;
    }

    public void ExitGameScene()
    {
        Application.Quit();
    }

    public void CheckIfGameCompleted()
    {
        if (playModes.isEndlessGame)
        {
            if (player.scoredFloatingObjects == numOfInstantiedRings)
            {
                floatingObjectSpawner.InstantiateObjects();
            }
        }

        if (playModes.isTimeTrialGame)
        {
            if (playModes.isEndlessGame)
            {
                if (player.playingTime == 0)
                {
                    Invoke("EndGame", 2f);
                    CalculateCashReward();
                }
            }
            else
            {
                if (player.scoredFloatingObjects == numOfInstantiedRings)
                {
                    Invoke("EndGame", 2f);
                    CalculateCashReward();
                }
                else if (player.playingTime == 0)
                {
                    Invoke("EndGame", 2f);
                    print("Game Failed");
                }
            }
        }
        else
        {
            if (!playModes.isEndlessGame)
            {
                if (player.scoredFloatingObjects == numOfInstantiedRings)
                {
                    Invoke("EndGame", 2f);
                    CalculateCashReward();
                }
            }
        }
    }

    void CalculateCashReward()
    {
        if (player.scoredFloatingObjects != 0)
        {
            cashFromScoredFloatingObjs = player.scoredFloatingObjects * floatingObjMultiplingValue;
            if (playModes.isTimeTrialGame)
            {
                playingTimeThreshold = (int)(player.timer / 2f);
                playTime = (int)(player.timer - player.playingTime);
                float _cashFromPlayingTime = (((float)playingTimeThreshold / (float)playTime) * playingTimeCashThreshold);
                cashFromPlayingTime = (int)_cashFromPlayingTime;
            }
            else
            {
                playTime = 0;
                cashFromPlayingTime = 0;
            }
            totalCash = cashFromScoredFloatingObjs + cashFromPlayingTime;
            rewardedCash += totalCash;

            UpdateCashRewardPanel();
            cashRewardPanel.SetActive(true);

            SaveRewardedCash();
        }
    }

    void UpdateCashRewardPanel()
    {
        playingTimeRewardPanelText.text = string.Format("{0:00.00}", playTime);
        cashFromScoredFloatingObjsText.text = cashFromScoredFloatingObjs.ToString();

        scoredObjsRewardPanelText.text = player.scoredFloatingObjects.ToString();
        cashFromPlayingTimeText.text = cashFromPlayingTime.ToString();

        totalCashText.text = totalCash.ToString();
    }

    public void SaveRewardedCash()
    {
        DataSaveManager.SaveInt("RC", rewardedCash);
        rewardedCashText.text = rewardedCash.ToString();
    }

    void LoadGame()
    {
        wallpapersMenu.LoadWallpapersMenu();
    }

    void SetUIBasedOnPlayMode()
    {
        //If EndlessGame
        numOfObjects.gameObject.SetActive(!playModes.isEndlessGame);
        numOfObjects.transform.parent.GetChild(1).gameObject.SetActive(!playModes.isEndlessGame); //slash text gameobject

        //If TrialGame
        player.playingTimeText.transform.parent.gameObject.SetActive(playModes.isTimeTrialGame);
        playingTimeRewardPanelText.transform.parent.gameObject.SetActive(playModes.isTimeTrialGame);

        if (playModes.isSpecificRingWithConeMode)
        {
            specificModeHint.SetActive(true);
        }
        else
        {
            specificModeHint.SetActive(false);
        }
    }

    public void GetNumerOfInsatiatedRings()
    {
        if (playModes.isSpecificRingWithConeMode && !playModes.isTwoCones)
        {
            numOfInstantiedRings += playModes.specificRingCount;
            numOfObjects.text = playModes.specificRingCount.ToString();
        }
        else
        {
            numOfInstantiedRings += floatingObjectSpawner.numberOfObjects;
        }
    }

    void ShowAdIfPossible()
    {
        gameSessions++;

        if(gameSessions >= 3)
        {
            unityAds.ShowVideoAd();
        }
    }

    void ShowFirstTimePlayingHint() //Called In Awake Function.
    {
        firstTimePlayingHint.SetActive(true);

        DataSaveManager.SaveBoolean("FTPH", true);
    }

    public void RateTheGame()
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.BanterGames.BouncyRings");
    }

    public void RateTheGameNow() //Called Form RateTheGame Panel.
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.BanterGames.BouncyRings");

        DataSaveManager.SaveBoolean("GR", true);
        rateTheGamePanel.SetActive(false);
    }

    public void NeverRateTheGame()
    {
        DataSaveManager.SaveBoolean("GR", true);
        rateTheGamePanel.SetActive(false);
    }

    public void LaterRateTheGame()
    {
        DataSaveManager.SaveBoolean("GR", false);
        rateTheGamePanel.SetActive(false);
    }

    void CheckIfRatePanelShoudPopUp()
    {
        if (!DataSaveManager.LoadBoolean("GR"))
        {
            if (!isRatePanelPopedUpToday)
            {
                if (challengeLevels.challengeLevelIndex > 4)
                {
                    rateTheGamePanel.SetActive(true);
                    isRatePanelPopedUpToday = true;
                }
            }
        }

    }

    public void OpenPrivacyPolicy()
    {
        Application.OpenURL("https://bantergames.blogspot.com/p/blog-page_19.html");
    }
}