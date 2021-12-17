using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanelNavigation : MonoBehaviour
{
    public GameObject panelToOpen;
    GameObject panelToClose;

    void Awake()
    {
        panelToClose = gameObject;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            NavigateBetweenPanels();
        }
    }

    public void NavigateBetweenPanels()
    {
        panelToClose.SetActive(false);

        panelToOpen.SetActive(true);
    }
}
