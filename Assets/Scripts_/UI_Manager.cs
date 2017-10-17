using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Manager : MonoBehaviour
{

    public GameObject LoadPanel;

    private void Start()
    {
        LoadPanel.gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    public void OnStart()
    {
        SceneManager.LoadSceneAsync(1);
        LoadPanel.gameObject.SetActive(true); 

        StartCoroutine(LoadNewScene());
    }

    IEnumerator LoadNewScene()
    {
        yield return new WaitForSeconds(1f);

        AsyncOperation async = SceneManager.LoadSceneAsync(1);

        while (!async.isDone)
        {
            yield return null;
        }
    }
}
