using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class loadingAfterCutScene : MonoBehaviour
{
    private float timer = 0;
    public GameObject obj;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 38)
        {
            obj.SetActive(true);
        }
    }
    public void LoadMap()
    {
        SceneManager.LoadScene("PlayerSelection");
    }
}
