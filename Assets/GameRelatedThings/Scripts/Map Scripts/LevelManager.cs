using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    int lvlUnlocked;
    public GameObject[] buttons;

    void Start()
    {
        lvlUnlocked = PlayerPrefs.GetInt("lvlUnlocked");

        for (int i  = 0; i < buttons.Length; i++)
        {
            if (i + 5 < lvlUnlocked)
                buttons[i].SetActive(true);
        }
    }
}
