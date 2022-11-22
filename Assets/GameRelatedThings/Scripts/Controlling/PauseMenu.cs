using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GamePaused = false;
    public GameObject pauseMenuWindow;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GamePaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }   

    public void Resume()
    {
        pauseMenuWindow.SetActive(false);
        Time.timeScale = 1f;
        GamePaused = false;
    }

    void Pause()
    {
        pauseMenuWindow.SetActive(true);
        Time.timeScale = 0f;
        GamePaused = true;
    }

    public void QuitLevel()
    {
        SceneManager.LoadScene("map");
        Time.timeScale = 1f;
        GamePaused = false;
    }
}
