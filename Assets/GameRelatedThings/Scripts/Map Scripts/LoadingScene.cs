using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    //Objectum async bet�lt�s eset�n!
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
    // Az al�bbi k�d az async bet�lt�st val�s�tja meg.
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

    // Az al�bbi k�dsor egy szimul�ci� a scenek k�z�tti bet�lt�snek, mivel az async bet�lt�s a pillanat t�red r�sze
    // alatt bet�lt�tt ez�rt egy szimul�ci�val lett helyetess�tve

}
