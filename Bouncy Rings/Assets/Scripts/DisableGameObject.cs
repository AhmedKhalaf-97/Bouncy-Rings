using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableGameObject : MonoBehaviour
{
    public float timer = 5f;

    void OnEnable()
    {
        Invoke("DisableGO", timer);
    }

    void DisableGO()
    {
        gameObject.SetActive(false);
    }
}
