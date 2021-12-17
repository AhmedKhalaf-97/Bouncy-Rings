using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrialsPanel : MonoBehaviour
{
    public PlayModes playmodes;
    public Player player;
    public FloatingObjectSpawner floatingObjectSpawner;

    public Toggle endlessToggle;
    public Toggle timeTrialToggle;
    public Toggle twoConesToggle;
    public Toggle classicToggle;

    public Slider playTimerSlider;
    public Text playTimerValueText;

    public Slider ringsCountSlider;
    public Text ringsCountText;

    void OnEnable()
    {
        endlessToggle.isOn = playmodes.isEndlessGame;
        timeTrialToggle.isOn = playmodes.isTimeTrialGame;
        twoConesToggle.isOn = playmodes.isTwoCones;
        classicToggle.isOn = !playmodes.isSpecificRingWithConeMode;

        playTimerSlider.value = player.timer;
        playTimerValueText.text = player.timer.ToString();

        ringsCountSlider.value = floatingObjectSpawner.numberOfObjects;
        ringsCountText.text = floatingObjectSpawner.numberOfObjects.ToString();
    }

    public void IsEndlessGame()
    {
        playmodes.isEndlessGame = endlessToggle.isOn;
    }

    public void IsTimeTrialGame()
    {
        playmodes.isTimeTrialGame = timeTrialToggle.isOn;
    }

    public void IsTwoConesGame()
    {
        playmodes.isTwoCones = twoConesToggle.isOn;
    }

    public void IsClassicGame()
    {
        playmodes.isSpecificRingWithConeMode = !classicToggle.isOn;
    }

    public void PlayTimerValue()
    {
        player.timer = playTimerSlider.value;
        playTimerValueText.text = player.timer.ToString();
    }

    public void RingsCount()
    {
        floatingObjectSpawner.numberOfObjects = (int)ringsCountSlider.value;
        ringsCountText.text = floatingObjectSpawner.numberOfObjects.ToString();
    }
}
