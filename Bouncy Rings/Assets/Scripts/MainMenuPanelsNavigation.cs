using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuPanelsNavigation : MonoBehaviour
{
    public GameObject hudCanvas;
    public GameObject mainMenuPanel;
    public GameObject[] subMainMenuPanels;
    public GameObject exitPanel;

    bool subPanelIsActive;

    [Header("Audio Stuff")]
    public AudioClip audioClip;
    public AudioSource audioSource;
    public GameObject[] subMainMenuPanelsDontNeedAudio;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            NavigateBetweenPanels();
        }
    }

    void NavigateBetweenPanels()
    {
        foreach (GameObject panel in subMainMenuPanels)
        {
            if (panel.activeInHierarchy)
            {
                subPanelIsActive = true;
                break;
            }
            else
            {
                subPanelIsActive = false;
            }
        }

        if (!subPanelIsActive)
        {
            if (exitPanel.GetComponent<ClosePanelWithAnimation>() != null)
            {
                if (!exitPanel.activeSelf == false)
                {
                    exitPanel.GetComponent<ClosePanelWithAnimation>().ClosePanel();
                }
                else
                {
                    if (!hudCanvas.activeInHierarchy)
                    {
                        exitPanel.SetActive(true);
                    }
                }
            }
            else
            {
                exitPanel.SetActive(!exitPanel.activeSelf);
            }
        }
        else
        {
            CloseSubMainMenuPanel();
        }
    }

    void CloseSubMainMenuPanel()
    {
        if (subPanelIsActive)
        {
            foreach (GameObject panel in subMainMenuPanels)
            {
                if (panel.activeInHierarchy)
                {
                    if (panel.GetComponent<ClosePanelWithAnimation>() != null)
                    {
                        panel.GetComponent<ClosePanelWithAnimation>().ClosePanel();
                    }
                    else
                    {
                        panel.SetActive(false);
                    }
                }

                foreach (GameObject _panel in subMainMenuPanelsDontNeedAudio)
                {
                    if(_panel != panel)
                    {
                        PlayAudioClip();
                    }
                }
            }
        }
    }

    void PlayAudioClip()
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }
}