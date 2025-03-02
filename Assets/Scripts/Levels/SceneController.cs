using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController sceneInstance;
    [SerializeField] Animator transitionAnimator;

    private void Awake()
    {
        if (sceneInstance == null)
        {
            sceneInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void GoNextScene()
    {
        StartCoroutine(LoadNextScene());
    }

    IEnumerator LoadNextScene()
    {
        transitionAnimator.SetTrigger("End");
        yield return new WaitForSeconds(1);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        transitionAnimator.SetTrigger("Start");
    }

    public void GoSpecifiedScene(string sceneName)
    {
        StartCoroutine(LoadSpecifiedScene(sceneName));
    }
    
    IEnumerator LoadSpecifiedScene(string sceneName)
    {
        transitionAnimator.SetTrigger("End");
        yield return new WaitForSeconds(1);
        SceneManager.LoadSceneAsync(sceneName);
        transitionAnimator.SetTrigger("Start");
    }
}