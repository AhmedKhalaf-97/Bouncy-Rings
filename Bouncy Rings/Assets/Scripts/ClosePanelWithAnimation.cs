using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosePanelWithAnimation : MonoBehaviour
{
    float animationTimeLengthInSeconds;

    Animator _animator;
    GameObject _gameObject;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _gameObject = gameObject;

        animationTimeLengthInSeconds = _animator.runtimeAnimatorController.animationClips[1].length;
    }

    public void ClosePanel()
    {
        _animator.SetTrigger("Close");
        Invoke("SetGameObjectToFalse", animationTimeLengthInSeconds);
    }

    void SetGameObjectToFalse()
    {
        _gameObject.SetActive(false);
    }
}
