using System;
using System.Collections;
using UnityEngine;

public class NetworkAvailability : MonoBehaviour
{
    public GameObject networkNotAvailablePanel;

    bool isConnected;

    public static NetworkAvailability instance;

    void Start()
    {
        instance = this;

        StartCoroutine(CheckInternetConnection());
    }

    public bool IsConnected()
    {
        StartCoroutine(CheckInternetConnection());

        if (!isConnected)
        {
            networkNotAvailablePanel.SetActive(true);
        }

        return isConnected;
    }

    IEnumerator CheckInternetConnection()
    {
        WWW www = new WWW("http://google.com");
        yield return www;

        if (www.error != null)
        {
            isConnected = false;
        }
        else
        {
            isConnected = true;
        }
    }
}
