using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    //Objectum async betöltés esetén!
    public GameObject LoadingScreen;
    public Image LoadingBarFill;

    float timer = 0.0f;
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneSimulation(sceneName));
    }
    IEnumerator LoadSceneSimulation(string sceneName)
    {
        LoadingScreen.SetActive(true);
        while (timer <= 1.0f)
        {
            timer += 0.2f;
            yield return new WaitForSeconds(0.3f);
            float progressValue = timer;
            LoadingBarFill.fillAmount = progressValue;
        }
        SceneManager.LoadSceneAsync(sceneName);
    }
    // Az alábbi kód az async betöltést valósítja meg.
    //IEnumerator LoadSceneAsync(string sceneName)
    //{


    //AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
    //LoadingScreen.SetActive(true);

    //while (!asyncOperation.isDone)
    //{
    //    float progressValue = Mathf.Clamp01(asyncOperation.progress / 0.9f);

    //    LoadingBarFill.fillAmount = progressValue;

    //    yield return null;
    //}
    //}

    // Az alábbi kódsor egy szimuláció a scenek közötti betöltésnek, mivel az async betöltés a pillanat töred része
    // alatt betöltött ezért egy szimulációval lett helyetessítve

}
