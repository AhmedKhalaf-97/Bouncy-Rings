using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeLevels : MonoBehaviour
{
    [Header("Challenge Setting Variables")]
    public int indexOfEmpircalLevelToMove;

    [Space]
    public int wantedChallengesNumber = 100;
    public int ringsCount = 6;
    public int startingRandomScore = 60;
    public int startingRequiredRings = 3;

    [Space]
    public Text empiricalChallengeIndexText;
    public Text empiricalChallenheNameText;

    [Space]
    public List<ChallengeLevel> empircalChallengeLevels = new List<ChallengeLevel>();

    [Header("Scripts")]
    public Player player;
    public MainMenu mainMenu;
    public PlayModes playModes;
    public FloatingObjectSpawner floatingObjectSpawner;

    [Header("UI Stuff")]
    public Text challengeHeaderText;
    public Text challengeNameText;
    public Text currentRunProgressText;
    public Text runTimesProgressText;
    public Slider progressSlider;
    public GameObject seenImageGO;
    public GameObject noEnoughCashPanel;
    public Text challengeCompletedIndexText;
    public GameObject challengeCompletedPanel;

    [Header("Skip Challenge")]
    public int skipPrice;
    public Text skipPriceText;

    [Header("Challenge Index")]
    public int challengeLevelIndex;
    public int partiallyLevelIndex;

    public int magnetCountBeforeChallenge;

    [Header("Challenge Levels")]
    public List<ChallengeLevel> challengeLevels = new List<ChallengeLevel>();

    [System.Serializable]
    public class ChallengeLevel
    {
        public string challengeName;

        [Header("Requirments : ")]
        public bool isTheExactRequirment;
        public bool isAbleToUseMagnets;
        [Header("Required Score")]
        public int requiredScore;
        [Header("Required Rings")]
        public int requiredRings;
        [Header("Between Min and Max Score")]
        public int minRequiredScore;
        public int maxRequiredScore;
        [Header("Between Min and Max Rings")]
        public int minRequiredRings;
        public int maxRequiredRings;
        [Header("Run Times : ")]
        public int runTimes;
        public bool isItRowRunTimes;

        [Header("Mode Configuration : ")]
        public bool isEndlessGame;
        public bool isTimeTrialGame;
        public bool isTwoCones;
        public bool isSpecificConeMode;
        public bool isAutoSetPlayTimer;
        public float playTimer;
        public int ringsCount;
    }

    void Awake()
    {
        LoadChallengeProgress();
    }

    public void SetChallengeLevel()
    {
        var challengeLevel = challengeLevels[challengeLevelIndex];

        playModes.isEndlessGame = challengeLevel.isEndlessGame;
        playModes.isTimeTrialGame = challengeLevel.isTimeTrialGame;
        playModes.isTwoCones = challengeLevel.isTwoCones;
        playModes.isSpecificRingWithConeMode = challengeLevel.isSpecificConeMode;
        playModes.isAutoSetPlayTimer = challengeLevel.isAutoSetPlayTimer;
        playModes.playTimer = challengeLevel.playTimer;

        floatingObjectSpawner.numberOfObjects = challengeLevel.ringsCount;

        magnetCountBeforeChallenge = player.magnetCount;
    }

    public void CheckIfChallengeCompleted()
    {
        var challengeLevel = challengeLevels[challengeLevelIndex];

        if (!challengeLevel.isAbleToUseMagnets)
        {
            if (player.magnetCount != magnetCountBeforeChallenge)
            {
                if (challengeLevel.runTimes > 1 && challengeLevel.isItRowRunTimes)
                {
                    partiallyLevelIndex = 0;
                }
                return;
            }
        }

        if (challengeLevel.isTheExactRequirment)
        {
            if (challengeLevel.requiredScore != 0)
            {
                if (mainMenu.latestScore == challengeLevel.requiredScore)
                {
                    CheckAgainIfAllRunTimesCompleted();
                }
                else
                {
                    if (challengeLevel.runTimes > 1 && challengeLevel.isItRowRunTimes)
                    {
                        partiallyLevelIndex = 0;
                    }
                }
            }

            if (challengeLevel.requiredRings != 0)
            {
                if (player.scoredFloatingObjects == challengeLevel.requiredRings)
                {
                    CheckAgainIfAllRunTimesCompleted();
                }
                else
                {
                    if (challengeLevel.runTimes > 1 && challengeLevel.isItRowRunTimes)
                    {
                        partiallyLevelIndex = 0;
                    }
                }
            }
        }
        else
        {
            if (challengeLevel.requiredScore != 0)
            {
                if (mainMenu.latestScore >= challengeLevel.requiredScore)
                {
                    CheckAgainIfAllRunTimesCompleted();
                }
                else
                {
                    if (challengeLevel.runTimes > 1 && challengeLevel.isItRowRunTimes)
                    {
                        partiallyLevelIndex = 0;
                    }
                }
            }

            if (challengeLevel.requiredRings != 0)
            {
                if (player.scoredFloatingObjects >= challengeLevel.requiredRings)
                {
                    CheckAgainIfAllRunTimesCompleted();
                }
                else
                {
                    if (challengeLevel.runTimes > 1 && challengeLevel.isItRowRunTimes)
                    {
                        partiallyLevelIndex = 0;
                    }
                }
            }
        }

        if (challengeLevel.minRequiredScore != 0 && challengeLevel.maxRequiredScore != 0)
        {
            if (challengeLevel.minRequiredScore <= mainMenu.latestScore && mainMenu.latestScore <= challengeLevel.maxRequiredScore)
            {
                CheckAgainIfAllRunTimesCompleted();
            }
            else
            {
                if (challengeLevel.runTimes > 1 && challengeLevel.isItRowRunTimes)
                {
                    partiallyLevelIndex = 0;
                }
            }
        }

        if (challengeLevel.minRequiredRings != 0 && challengeLevel.maxRequiredRings != 0)
        {
            if (challengeLevel.minRequiredRings <= player.scoredFloatingObjects && player.scoredFloatingObjects <= challengeLevel.maxRequiredRings)
            {
                CheckAgainIfAllRunTimesCompleted();
            }
            else
            {
                if (challengeLevel.runTimes > 1 && challengeLevel.isItRowRunTimes)
                {
                    partiallyLevelIndex = 0;
                }
            }
        }
    }

    void CheckAgainIfAllRunTimesCompleted()
    {
        var challengeLevel = challengeLevels[challengeLevelIndex];

        if (challengeLevel.runTimes == 1)
        {
            ChallengeCompleted();
            return;
        }

        if (challengeLevel.runTimes > 1)
        {
            partiallyLevelIndex++;

            if (challengeLevel.runTimes == partiallyLevelIndex)
            {
                ChallengeCompleted();
                return;
            }
        }
    }

    void ChallengeCompleted()
    {
        challengeCompletedIndexText.text = "Challenge " + (challengeLevelIndex + 1) + " Completed";
        challengeCompletedPanel.SetActive(true);

        if (challengeLevelIndex < (challengeLevels.Count - 1))
        {
            challengeLevelIndex++;
            IsNextChallengeSeen(false);
        }
        partiallyLevelIndex = 0;
        SaveChallengeProgress();
    }

    public void UpdateChallengeUI() //Called on ChallengeInfo button click event.
    {
        var challengeLevel = challengeLevels[challengeLevelIndex];

        challengeHeaderText.text = "Challenge " + (challengeLevelIndex + 1) + " / " + challengeLevels.Count;
        challengeNameText.text = challengeLevel.challengeName;
        currentRunProgressText.text = partiallyLevelIndex.ToString();
        runTimesProgressText.text = challengeLevel.runTimes.ToString();
        progressSlider.maxValue = challengeLevel.runTimes;
        progressSlider.value = partiallyLevelIndex;

        IsNextChallengeSeen(true);
    }

    void IsNextChallengeSeen(bool status)
    {
        seenImageGO.SetActive(!status);
    }

    public void SkipChallenge()
    {
        if (skipPrice <= mainMenu.rewardedCash)
        {
            ChallengeCompleted();
            UpdateChallengeUI();

            mainMenu.rewardedCash -= skipPrice;
            mainMenu.SaveRewardedCash();
        }
        else
        {
            DoNotHaveEnoughCash();
        }
    }

    public void UpdateSkipChallengeUI()
    {
        skipPriceText.text = "Skip For " + skipPrice;
    }

    void SaveChallengeProgress()
    {
        DataSaveManager.SaveInt("CI", challengeLevelIndex);
    }

    void LoadChallengeProgress()
    {
        if (DataSaveManager.IsDataExist("CI"))
        {
            challengeLevelIndex = DataSaveManager.LoadInt("CI");
        }
    }

    void DoNotHaveEnoughCash()
    {
        StartCoroutine(NoEnoughCashPanel());
    }

    IEnumerator NoEnoughCashPanel()
    {
        noEnoughCashPanel.SetActive(true);
        yield return new WaitForSeconds(1f);
        noEnoughCashPanel.SetActive(false);
    }

    void GenerateRandomChallenge()
    {
        int challengeLevelCount = empircalChallengeLevels.Count;
        int neededChallengeLevelsCount = (empircalChallengeLevels.Count + wantedChallengesNumber);

        for (int i = challengeLevelCount; i < neededChallengeLevelsCount; i++)
        {
            empircalChallengeLevels.Add(new ChallengeLevel());
            var challengeLevel = empircalChallengeLevels[i];

            int randomIndex = Random.Range(1, 5);

            challengeLevel.isTheExactRequirment = RandomBoolean(false, 70);
            challengeLevel.isAbleToUseMagnets = RandomBoolean(true, 70);

            challengeLevel.runTimes = Random.Range(1, 4);
            if (challengeLevel.runTimes > 1)
            {
                challengeLevel.isItRowRunTimes = RandomBoolean();
            }

            challengeLevel.isEndlessGame = true;
            challengeLevel.isTimeTrialGame = RandomBoolean(false, 70);
            challengeLevel.isTwoCones = RandomBoolean();
            challengeLevel.isSpecificConeMode = RandomBoolean(false, 40);
            challengeLevel.isAutoSetPlayTimer = false;
            challengeLevel.ringsCount = ringsCount;

            switch (randomIndex)
            {
                case 1:
                    challengeLevel.requiredScore = RandomScore();
                    challengeLevel.playTimer = (((challengeLevel.requiredScore / 30) * 80) / 20);

                    SetChallengeName(i);
                    break;
                case 2:
                    challengeLevel.requiredRings = Random.Range(startingRequiredRings, (startingRequiredRings + 4));
                    challengeLevel.playTimer = ((challengeLevel.requiredRings * 80) / 20);

                    SetChallengeName(i);
                    break;
                case 3:
                    challengeLevel.minRequiredScore = RandomScore();
                    challengeLevel.maxRequiredScore = (challengeLevel.minRequiredScore + 60);
                    challengeLevel.playTimer = (((challengeLevel.maxRequiredScore / 30) * 80) / 20);
                    if (challengeLevel.minRequiredScore != 0)
                    {
                        challengeLevel.isTheExactRequirment = false;
                    }

                    SetChallengeName(i);
                    break;
                case 4:
                    challengeLevel.minRequiredRings = Random.Range(startingRequiredRings, (startingRequiredRings + 4));
                    challengeLevel.maxRequiredRings = (challengeLevel.minRequiredRings + 3);
                    challengeLevel.playTimer = ((challengeLevel.maxRequiredRings * 80) / 20);
                    if (challengeLevel.minRequiredRings != 0)
                    {
                        challengeLevel.isTheExactRequirment = false;
                    }

                    SetChallengeName(i);
                    break;
            }
        }
    }

    void SetChallengeName(int i)
    {
        string scoreOrRings = "";
        string exactly = "";
        string magnets = "";
        string requirment = "";
        string runs = "";
        string timer = "";

        var challengeLevel = empircalChallengeLevels[i];

        if (challengeLevel.isTheExactRequirment)
        {
            exactly = "Exactly ";
        }

        if (!challengeLevel.isAbleToUseMagnets)
        {
            magnets = " Without Using Magnets";
        }

        if (challengeLevel.isTimeTrialGame)
        {
            timer = " Within " + challengeLevel.playTimer + "s";
        }

        if (challengeLevel.requiredScore != 0 || challengeLevel.minRequiredScore != 0)
        {
            scoreOrRings = "Score ";
        }

        if (challengeLevel.requiredRings != 0 || challengeLevel.minRequiredRings != 0)
        {
            scoreOrRings = "Put ";
        }

        if (challengeLevel.requiredScore != 0)
        {
            requirment = challengeLevel.requiredScore + timer + " In";
        }

        if (challengeLevel.requiredRings != 0)
        {
            requirment = challengeLevel.requiredRings + " Rings In The Cone" + timer + " In";
        }

        if (challengeLevel.minRequiredScore != 0)
        {
            requirment = "Between " + challengeLevel.minRequiredScore + " And " + challengeLevel.maxRequiredScore + timer + " In";
        }

        if (challengeLevel.minRequiredRings != 0)
        {
            requirment = "Between " + challengeLevel.minRequiredRings + " And " + challengeLevel.maxRequiredRings + " Rings In The Cone" + timer + " In";
        }

        if (challengeLevel.runTimes == 1)
        {
            runs = " x1 Run";
        }

        if (challengeLevel.runTimes > 1 && challengeLevel.isItRowRunTimes)
        {
            runs = " x" + challengeLevel.runTimes + " Runs A Row";
        }

        if (challengeLevel.runTimes > 1 && !challengeLevel.isItRowRunTimes)
        {
            runs = " x" + challengeLevel.runTimes + " Runs";
        }

        challengeLevel.challengeName = scoreOrRings + exactly + requirment + runs + magnets;
    }

    void MoveEmpircalLevelToChallengeLevelsList()
    {
        challengeLevels.Add(empircalChallengeLevels[indexOfEmpircalLevelToMove]);

        empircalChallengeLevels.RemoveAt(indexOfEmpircalLevelToMove);
    }

    void GenerationEmpiricalChallengesAndSettingItInputs()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            empircalChallengeLevels.Clear();
            indexOfEmpircalLevelToMove = 0;

            GenerateRandomChallenge();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            MoveEmpircalLevelToChallengeLevelsList();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            print(empircalChallengeLevels[indexOfEmpircalLevelToMove].challengeName);
        }

        if (empircalChallengeLevels.Count > 0)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (indexOfEmpircalLevelToMove < (empircalChallengeLevels.Count - 1))
                {
                    indexOfEmpircalLevelToMove++;
                }
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (indexOfEmpircalLevelToMove > 0)
                {
                    indexOfEmpircalLevelToMove--;
                }
            }

            empiricalChallengeIndexText.text = indexOfEmpircalLevelToMove.ToString();
            empiricalChallenheNameText.text = empircalChallengeLevels[indexOfEmpircalLevelToMove].challengeName;
        }
    }

    int RandomScore()
    {
        int randomIndex = Random.Range(1, 4);
        int data = startingRandomScore;

        switch (randomIndex)
        {
            case 3:
                data = 90;
                break;
            case 2:
                data = data + (30 * 1);
                break;
            case 1:
                data = data + (30 * 2);
                break;
        }

        return data;
    }

    bool RandomBoolean()
    {
        int randomIndex = Random.Range(0, 2);

        if (randomIndex == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool RandomBoolean(bool conditionNeeded, int conditionPossibilityPercentage)
    {
        int randomIndex = Random.Range(0, 100);

        if (randomIndex < conditionPossibilityPercentage)
        {
            return conditionNeeded;
        }
        else
        {
            return !conditionNeeded;
        }
    }

    //void Update()
    //{
    //    //GenerationEmpiricalChallengesAndSettingItInputs();

    //    //if (Input.GetKeyDown(KeyCode.Space))
    //    //{
    //    //    GetStatisticsAboutLevels();
    //    //}

    //    //if (Input.GetKeyDown(KeyCode.P))
    //    //{
    //    //    PrintTwoConesMode();
    //    //}

    //    //if (Input.GetKeyDown(KeyCode.S))
    //    //{
    //    //    SetTwoConesMode();
    //    //}
    //}

    //void GetStatisticsAboutLevels()
    //{
    //    int timeTrialModeCount = 0;
    //    int noTimeTrialModeCount = 0;
    //    int twoConesModeCount = 0;
    //    int oneConeModeCount = 0;
    //    int specificConeModeCount = 0;
    //    int classicConeModeCount = 0;

    //    for (int i = 0; i < challengeLevels.Count; i++)
    //    {
    //        if (challengeLevels[i].isSpecificConeMode)
    //        {
    //            specificConeModeCount++;
    //        }
    //        else
    //        {
    //            classicConeModeCount++;
    //        }

    //        if (challengeLevels[i].isTimeTrialGame)
    //        {
    //            timeTrialModeCount++;
    //        }
    //        else
    //        {
    //            noTimeTrialModeCount++;
    //        }

    //        if (challengeLevels[i].isTwoCones)
    //        {
    //            twoConesModeCount++;
    //        }
    //        else
    //        {
    //            oneConeModeCount++;
    //        }
    //    }

    //    print("Specific : " + specificConeModeCount);
    //    print("Classical : " + classicConeModeCount);
    //    print("TimeTrial : " + timeTrialModeCount);
    //    print("NoTimeTrial : " + noTimeTrialModeCount);
    //    print("TwoCones : " + twoConesModeCount);
    //    print("OneCone : " + oneConeModeCount);
    //}

    //void PrintTwoConesMode()
    //{
    //    for (int i = 0; i < challengeLevels.Count; i++)
    //    {
    //        print(i + " : " + challengeLevels[i].isTwoCones);
    //    }
    //}

    //void SetTwoConesMode()
    //{

    //    for (int i = 0; i < challengeLevels.Count; i++)
    //    {
    //        challengeLevels[i].isTwoCones = true;
    //    }

    //    for (int i = 0; i < challengeLevels.Count; i++)
    //    {
    //        if(i %  Random.Range(2,4) == 1)
    //        {
    //            challengeLevels[i].isTwoCones = false;
    //        }
    //    }
    //}
}
