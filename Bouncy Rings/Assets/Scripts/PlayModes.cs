using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayModes : MonoBehaviour
{
    public bool isEndlessGame; //Determine Instantiated Rings.
    public bool isTimeTrialGame; //Determine Playing Time.
    public bool isAutoSetPlayTimer = true;
    public float playTimer = 20f;

    [Header("Playing Mode Variables")]
    public bool isTwoCones;
    public bool isSpecificRingWithConeMode;

    [Header("Specific Ring With Cone")]
    public int percentageOfSpecificRingColor = 50;
    public int instantiatedRingsCount;

    public int specificRingCount;
    public Color specificColor;

    [Header("Cones")]
    public Transform conesTransform;
    public MeshRenderer oneConeRenderer;
    public MeshRenderer[] twoConeRenderers;

    [Header("Mode Configuration Stuff")]
    public Button twoConesButton;
    public Button oneConeButton;
    public Button classicalModeButton;
    public Button specificModeButton;
    public Slider ringsCountSlider;
    public Text sliderValueText;

    [Header("Scripts")]
    public FloatingObjectSpawner floatingObjectSpawner;
    public Player player;

    Color GetRandomColor()
    {
        Color color;
        color.r = Random.Range(0f, 1f);
        color.g = Random.Range(0f, 1f);
        color.b = Random.Range(0f, 1f);
        color.a = 1f;

        return color;
    }

    public void SetTheScene()
    {
        ApplyConesType();
        GiveConesColor();
    }

    void ApplyConesType()
    {
        conesTransform.GetChild(0).gameObject.SetActive(isTwoCones);
        conesTransform.GetChild(1).gameObject.SetActive(!isTwoCones);
    }

    public void GiveRingColors(GameObject ringGO, int numOfRings)
    {
        if (isSpecificRingWithConeMode)
        {
            specificRingCount = ((numOfRings * percentageOfSpecificRingColor) / 100);
            instantiatedRingsCount++;

            if(instantiatedRingsCount <= specificRingCount)
            {
                ringGO.GetComponent<MeshRenderer>().material.color = specificColor;
            }
            else
            {
                ringGO.GetComponent<MeshRenderer>().material.color = GetRandomColor();
            }
        }
        else //Random Colors.
        {
            ringGO.GetComponent<MeshRenderer>().material.color = GetRandomColor();
        }
    }

    void GiveConesColor()
    {
        if (isSpecificRingWithConeMode)
        {
            specificColor = GetRandomColor();

            if (isTwoCones)
            {
                twoConeRenderers[0].material.color = specificColor;
            }
            else
            {
                oneConeRenderer.material.color = specificColor;
            }
        }
        else //Random Colors.
        {
            if (isTwoCones)
            {
                Color color = GetRandomColor();
                foreach (MeshRenderer renderer in twoConeRenderers)
                {
                    renderer.material.color = color;
                }
            }
            else
            {
                oneConeRenderer.material.color = GetRandomColor();
            }
        }
    }

    /*  Mode Configuration Stuff */

    public void IsTimeTrialGameMode(bool isTimeTrialMode)
    {
        isTimeTrialGame = isTimeTrialMode;
        isEndlessGame = !isTimeTrialMode;

        ResetModeConfigurationOnEnable();
    }

    public void IsTwoConesScene(bool isTwoConesMode)
    {
        isTwoCones = isTwoConesMode;

        twoConesButton.interactable = !isTwoConesMode;
        oneConeButton.interactable = isTwoConesMode;

        twoConesButton.transform.GetChild(0).gameObject.SetActive(isTwoConesMode);
        oneConeButton.transform.GetChild(0).gameObject.SetActive(!isTwoConesMode);
    }

    public void IsSpecificRingConeScene(bool isSpecific)
    {
        isSpecificRingWithConeMode = isSpecific;

        specificModeButton.interactable = !isSpecific;
        classicalModeButton.interactable = isSpecific;

        specificModeButton.transform.GetChild(0).gameObject.SetActive(isSpecific);
        classicalModeButton.transform.GetChild(0).gameObject.SetActive(!isSpecific);
    }

    public void ShowSliderValueOfRingsCount()
    {
        if (ringsCountSlider.value % 2 == 1)
        {
            ringsCountSlider.value += 1;
        }
        sliderValueText.text = ringsCountSlider.value.ToString();
        floatingObjectSpawner.numberOfObjects = (int)ringsCountSlider.value;
    }

    public void SetPlayerTimer()
    {
        if (isTimeTrialGame && isAutoSetPlayTimer)
        {
            player.timer = ((floatingObjectSpawner.numberOfObjects * 80) / 20); //This equation depends on that 20 ring in 80s.
        }

        if(isTimeTrialGame && !isAutoSetPlayTimer)
        {
            player.timer = playTimer;
        }
    }

    void ResetModeConfigurationOnEnable()
    {
        isAutoSetPlayTimer = true;

        IsTwoConesScene(true);
        IsSpecificRingConeScene(false);
        ShowSliderValueOfRingsCount();
    }

    /*  Mode Configuration Stuff End */
}
