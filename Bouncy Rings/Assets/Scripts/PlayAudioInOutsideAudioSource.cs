using UnityEngine;
using UnityEngine.UI;

public class PlayAudioInOutsideAudioSource : MonoBehaviour
{
    public AudioClip audioClip;

    AudioSource outsideAudioSource;
    Button button;

    void Awake()
    {
        outsideAudioSource = GameObject.FindGameObjectWithTag("OutsideAudioSource").GetComponent<AudioSource>();
        button = GetComponent<Button>();

        button.onClick.AddListener(PlayAudioClip);
    }

    void PlayAudioClip()
    {
        outsideAudioSource.clip = audioClip;
        outsideAudioSource.Play();
    }
}
