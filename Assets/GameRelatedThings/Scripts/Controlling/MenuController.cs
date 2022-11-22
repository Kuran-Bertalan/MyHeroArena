using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public PlayerObject playerObject;

    public void SinglePlayerButton()
    {
        if(playerObject.getIsPlayerViewedCutscene() == 0)
        {
            SceneManager.LoadScene("Cutscene01");
        }
        else if (playerObject.getIsPlayerSelectedHero() == 1 && playerObject.getIsPlayerFinishedWithTutorial() == 1 && playerObject.getIsPlayerViewedCutscene() == 1)
        {
            SceneManager.LoadScene("map");
        }
        else
        {
            SceneManager.LoadScene("PlayerSelection");
        }
       
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    public void MultiplayerButton()
    {
        SceneManager.LoadScene("MultiplayerLoading");
    }

    #region Szintbetoltesek
    public void LoadLevel1()
    {
        SceneManager.LoadScene(3);
    }
    public void LoadLevel2()
    {
        SceneManager.LoadScene(4);
    }
    public void LoadLevel3()
    {
        SceneManager.LoadScene(5);
    }
    public void LoadLevel4()
    {
        SceneManager.LoadScene(6);
    }
    public void LoadLevel5()
    {
        SceneManager.LoadScene(7);
    }
    #endregion
}
