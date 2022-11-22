using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnlockNextLvl : MonoBehaviour
{
    public int unlockNextLvl;
    // Start is called before the first frame update
    void Start()
    {
        unlockNextLvl = SceneManager.GetActiveScene().buildIndex + 1;
    }
    public void LoadNextLevel()
    {
        if(SceneManager.GetActiveScene().name == "Tutorial")
        {
            PlayerPrefs.SetInt("isPlayerFinishedWithTutorial", 1);
        }

        if(unlockNextLvl > PlayerPrefs.GetInt("lvlUnlocked"))
        {
            PlayerPrefs.SetInt("lvlUnlocked", unlockNextLvl);
        }
        SceneManager.LoadScene("Map");
    }
}
