using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingScoreText : MonoBehaviour
{
    public float speed = 1.8f;
    public float duration = 2f;


    void Start()
    {
        Destroy(gameObject, duration);
    }

    void Update()
    {
        transform.Translate(Vector3.up * speed);
    }
}
