using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    public float logoTimer = 2f;

    void Awake()
    {
        Invoke("StartGameScene", logoTimer);
    }

    void StartGameScene()
    {
        SceneManager.LoadSceneAsync(1);
    }
}
