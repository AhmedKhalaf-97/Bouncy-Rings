using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    public MainMenu mainMenu;
    public PlayModes playModes;
    bool isScenePaused;

    public Explosion explosion;

    public Slider horizontalSlider;
    public Slider verticalSlider;



    public static List<int> floatingObjectsInstanceIds = new List<int>();

    [Header("Audio Stuff")]
    public AudioClip scoreAC;
    public AudioClip timeFinishedAC;
    public AudioSource audioSource;

    [Header("Audio Control")]
    public GameObject audioToggleButtonCross;
    bool isAudioOn = true;

    List<AudioSource> audioSources = new List<AudioSource>();

    [Header("Controll Stuff")]
    public GameObject touch2Controller;
    public Image touch1ButtonImage;
    public Image touch2ButtonImage;
    bool isTouch1Controller;

    public Slider accerometerSensitivity;
    public float thrustValue;

    public Transform howToPlayPanels;

    [Header("Graphics Quality")]
    public int qualityModeIndex;
    public Text qualityModeText;
    public Button leftSortButton;
    public Button rightSortButton;
    public RainCameraController rcc;
    public List<GraphicsQualityLevels> graphicsQualityLevels = new List<GraphicsQualityLevels>();

    [Header("PlayingData")]
    public float timer;
    public float playingTime;
    public Text playingTimeText;
    public int scoredFloatingObjects;
    public Text ScoredObjectsCountText;

    [Header("Buttons Not To Be Clicked When Shooting")]
    public GameObject[] buttons;

    [Header("Magnets")]
    public int magnetCount;
    public float magnetTimer;
    public Slider magnetTimerSlider;
    public Button magnetButton;
    public Text magnetCountText;

    float lerpSpeed = 0;
    Coroutine magnetCoroutine;
    public List<GameObject> magnetGameobjects = new List<GameObject>();

    [Header("Playing Instructions")]
    public Transform playingInstructionsTransform;

    [System.Serializable]
    public class GraphicsQualityLevels
    {
        public string qualityName;
        public WaterEffects waterEffects;
        public AntiAliasing antiAliasing;
    }

    public static float yAxisOffset;

    void OnEnable()
    {
        Invoke("SetYAxisAccelerationOffset", 2f);

        Invoke("GetMagnetGameobjects", 0.1f);
    }

    void Update()
    {
        CheckIfEscapeButtonPressed();
        SetAccelrometerSlider();

        foreach (GameObject gameObject in buttons)
        {
            if (EventSystem.current.currentSelectedGameObject == gameObject)
            {
                return;
            }
        }

        if (!Application.isMobilePlatform)
        {
            if (Input.GetMouseButtonDown(0))
            {
                explosion.Explode(0);
            }

            if (Input.GetMouseButtonDown(1))
            {
                explosion.Explode(1);
            }
        }
        else
        {
            if (isTouch1Controller)
            {
                if (Input.GetTouch(0).position.x < Screen.width * 0.5f)
                {
                    explosion.Explode(0);
                }
                else
                {
                    explosion.Explode(1);
                }
            }
        }
    }

    public void Controller(int buttonIndex)
    {
        explosion.Explode(buttonIndex);
    }

    public void ControllerTouchIndex(int touchIndex)
    {
        if(touchIndex == 0)
        {
            isTouch1Controller = true;
            touch2Controller.SetActive(false);

            Color t1 = touch1ButtonImage.color;
            t1.a = 1;
            touch1ButtonImage.color = t1;

            Color t2 = touch2ButtonImage.color;
            t2.a = 0.2f;
            touch2ButtonImage.color = t2;
        }

        if(touchIndex == 1)
        {
            isTouch1Controller = false;
            touch2Controller.SetActive(true);

            Color t1 = touch1ButtonImage.color;
            t1.a = 0.2f;
            touch1ButtonImage.color = t1;

            Color t2 = touch2ButtonImage.color;
            t2.a = 1f;
            touch2ButtonImage.color = t2;
        }

        SetHowToPlayPanels();

        DataSaveManager.SaveInt("TI", touchIndex);
    }

    void SetHowToPlayPanels()
    {
        howToPlayPanels.GetChild(0).gameObject.SetActive(isTouch1Controller);
        howToPlayPanels.GetChild(1).gameObject.SetActive(!isTouch1Controller);
    }

    public void EnablePlayingInstructions()
    {
        StartCoroutine(EnablePlayingInstructionsCoroutine());
    }

    IEnumerator EnablePlayingInstructionsCoroutine()
    {
        playingInstructionsTransform.gameObject.SetActive(true);
        playingInstructionsTransform.GetChild(0).gameObject.SetActive(isTouch1Controller);
        playingInstructionsTransform.GetChild(1).gameObject.SetActive(!isTouch1Controller);

        yield return new WaitForSeconds(0.085f);

        Time.timeScale = 0;
    }

    public void ContinueTheGame() //After playing intructions has been shown.
    {
        Time.timeScale = 1;
        playingInstructionsTransform.gameObject.SetActive(false);
    }

    public void SetYAxisAccelerationOffset()
    {
        yAxisOffset = Input.acceleration.y;
    }

    void SetAccelrometerSlider()
    {
        if (Application.isMobilePlatform)
        {
            horizontalSlider.value = Input.acceleration.x;
            verticalSlider.value = -(Input.acceleration.y - yAxisOffset);
        }
        else
        {
            horizontalSlider.value = Input.GetAxis("Mouse X");
            verticalSlider.value = Input.GetAxis("Mouse Y");
        }
    }

    public void SetThrustValueForFloatingObject()
    {
        thrustValue = accerometerSensitivity.value;
        DataSaveManager.SaveFloat("AS", thrustValue);
    }

    public void GraphicsQualitySorting(int buttonIndex)
    {
        if (buttonIndex == 1)
        {
            if(qualityModeIndex < graphicsQualityLevels.Count - 1)
            {
                qualityModeIndex++;
                GraphicsQualityLevel(qualityModeIndex);
            }
        }

        if(buttonIndex == 0)
        {
            if(qualityModeIndex == 0)
            {
                return;
            }
            qualityModeIndex--;
            GraphicsQualityLevel(qualityModeIndex);
        }

        if (qualityModeIndex == graphicsQualityLevels.Count - 1)
        {
            rightSortButton.interactable = false;
        }
        else
        {
            rightSortButton.interactable = true;
        }

        if (qualityModeIndex == 0)
        {
            leftSortButton.interactable = false;
        }
        else
        {
            leftSortButton.interactable = true;
        }

        DataSaveManager.SaveInt("QMI", qualityModeIndex);
    }

    void GraphicsQualityLevel(int levelIndex)
    {
        qualityModeText.text = graphicsQualityLevels[levelIndex].qualityName.ToString();

        int aa = (int)graphicsQualityLevels[levelIndex].antiAliasing;
        if (aa == 0)
        {
            QualitySettings.antiAliasing = 1;
        }
        if(aa == 1)
        {
            QualitySettings.antiAliasing = 2;
        }
        if(aa == 2)
        {
            QualitySettings.antiAliasing = 4;
        }
        if(aa == 3)
        {
            QualitySettings.antiAliasing = 8;
        }

        rcc.ShaderType = (RainDropTools.RainDropShaderType)graphicsQualityLevels[levelIndex].waterEffects;
    }

    public void LoadSettings()
    {
        GetAllAudioSources();

        ControllerTouchIndex(DataSaveManager.LoadInt("TI"));

        if (DataSaveManager.IsDataExist("AS"))
        {
            thrustValue = DataSaveManager.LoadFloat("AS");
            accerometerSensitivity.value = thrustValue;
        }

        if (DataSaveManager.IsDataExist("QMI"))
        {
            qualityModeIndex = DataSaveManager.LoadInt("QMI");
            GraphicsQualityLevel(qualityModeIndex);
        }

        if (DataSaveManager.IsDataExist("IAO"))
        {
            isAudioOn = DataSaveManager.LoadBoolean("IAO");
            ApplyAudioControlStatus();
        }
    }

    public void ResetToDefaultSettings()
    {
        ControllerTouchIndex(0);
        DataSaveManager.SaveInt("TI", 0);

        thrustValue = 30;
        accerometerSensitivity.value = thrustValue;
        DataSaveManager.SaveFloat("AS", thrustValue);

        qualityModeIndex = 3;
        GraphicsQualityLevel(qualityModeIndex);
        DataSaveManager.SaveInt("QMI", qualityModeIndex);
    }

    public void GetAllFloatingObjectsInstanceIds()
    {
        floatingObjectsInstanceIds.Clear();

        GameObject[] floatingObjects = GameObject.FindGameObjectsWithTag("Floating");
        foreach (GameObject go in floatingObjects)
        {
            floatingObjectsInstanceIds.Add(go.GetInstanceID());
        }
    }

    void CheckIfEscapeButtonPressed()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isScenePaused = mainMenu.pausePanel.activeSelf;
            mainMenu.PauseScene(!isScenePaused);
        }
    }

    public void CalculatePlayingTime(bool runningStatus)
    {
        if (playModes.isTimeTrialGame)
        {
            if (runningStatus)
            {
                StartCoroutine(CalculatePlayingTimeCoroutine());
            }
            else
            {
                StopCoroutine(CalculatePlayingTimeCoroutine());
            }
        }
    }

    IEnumerator CalculatePlayingTimeCoroutine()
    {
        playingTime = timer;

        while (true)
        {
            if(playingTime > 0)
            {
                playingTime -= Time.deltaTime;
                playingTimeText.text = string.Format("{0:00.00}", playingTime);
            }
            else
            {
                playingTime = 0;
                playingTimeText.text = string.Format("{0:00.00}", 0);
                PlayTimeFinishedAudioClip();
                mainMenu.CheckIfGameCompleted();
                yield break;
            }
            yield return null;
        }
    }

    void GetMagnetGameobjects()
    {
        magnetGameobjects.Clear();

        foreach (Magnet magnet in Resources.FindObjectsOfTypeAll(typeof(Magnet)) as Magnet[])
        {
#if UNITY_EDITOR
            if (EditorUtility.IsPersistent(magnet.transform.root.gameObject))
                continue;
#endif

            if (magnet.transform.parent.parent.gameObject.activeInHierarchy)
            {
                magnetGameobjects.Add(magnet.gameObject);
            }
        }
    }

    public void EnableMagnets()
    {
        if(magnetCount > 0)
        {
            magnetCount--;
            DataSaveManager.SaveInt("MC", magnetCount);
            UpdateMagnetUI();

            magnetButton.interactable = false;

            magnetCoroutine = StartCoroutine(EnableMagnetsCoroutine());
        }
    }

    IEnumerator EnableMagnetsCoroutine()
    {
        foreach (GameObject magnetGO in magnetGameobjects)
        {
            magnetGO.SetActive(true);
        }

        magnetTimerSlider.gameObject.SetActive(true);

        lerpSpeed = 0;

        magnetTimerSlider.minValue = 0;
        magnetTimerSlider.maxValue = magnetTimer;
        magnetTimerSlider.value = magnetTimer;

        while (true)
        {
            lerpSpeed += Time.deltaTime / magnetTimer;
            magnetTimerSlider.value = Mathf.Lerp(magnetTimer, 0, lerpSpeed);

            if (0 == magnetTimerSlider.value)
            {
                lerpSpeed = 0;
                magnetTimerSlider.gameObject.SetActive(false);

                foreach (GameObject magnetGO in magnetGameobjects)
                {
                    magnetGO.SetActive(false);
                }

                if (magnetCount > 0)
                {
                    magnetButton.interactable = true;
                }

                yield break;
            }
            yield return null;
        }
    }

    public void ResetMagnet() //After Reload or Start.
    {
        if (magnetCoroutine != null)
        {
            StopCoroutine(magnetCoroutine);
        }

        magnetTimerSlider.gameObject.SetActive(false);

        foreach (GameObject magnetGO in magnetGameobjects)
        {
            magnetGO.SetActive(false);
        }

        UpdateMagnetUI();
    }

    void UpdateMagnetUI()
    {
        magnetCountText.text = magnetCount.ToString();

        Color color = magnetCountText.color;

        if (magnetCount == 0)
        {
            magnetButton.interactable = false;

            color.a = 0.5f;
            magnetCountText.color = color;
        }

        if (magnetCount > 0)
        {
            magnetButton.interactable = true;

            color.a = 1f;
            magnetCountText.color = color;
        }
    }

    public void LoadMagnetCount() //Called In MainMenu Awake.
    {
        if (DataSaveManager.IsDataExist("MC"))
        {
            magnetCount = DataSaveManager.LoadInt("MC");
        }
    }

    public void PlayScoreAudioClip()
    {
        audioSource.clip = scoreAC;
        audioSource.Play();
    }

    void PlayTimeFinishedAudioClip()
    {
        audioSource.clip = timeFinishedAC;
        audioSource.Play();
    }

    public void ToggleAudioControll()
    {
        isAudioOn = !isAudioOn;

        ApplyAudioControlStatus();

        DataSaveManager.SaveBoolean("IAO", isAudioOn);
    }

    void ApplyAudioControlStatus()
    {
        audioToggleButtonCross.SetActive(!isAudioOn);

        foreach (AudioSource ac in audioSources)
        {
            ac.enabled = isAudioOn;
        }
    }

    void GetAllAudioSources()
    {
        audioSources.Clear();

        foreach (AudioSource _audioSource in Resources.FindObjectsOfTypeAll(typeof(AudioSource)) as AudioSource[])
        {
#if UNITY_EDITOR
            if (EditorUtility.IsPersistent(_audioSource.transform.root.gameObject))
                continue;
#endif

            audioSources.Add(_audioSource);
        }
    }
}

public enum WaterEffects
{
    Expensive,
    Cheap,
    NoDistortion
}

public enum AntiAliasing
{
    Disabled,
    low,
    medium,
    High
}
