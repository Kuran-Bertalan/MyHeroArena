using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterCreation : MonoBehaviour
{
    private GameObject[] characters;
    private int index;

    private void Start()
    {
        characters = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            characters[i] = transform.GetChild(i).gameObject;
        }

        foreach (GameObject go in characters)
        {
            go.SetActive(false);
        }

        if (characters[0])
            characters[0].SetActive(true);
    }

    public void SwitchBack()
    {
        characters[index].SetActive(false);

        index--;
        if (index < 0)
            index = characters.Length - 1;

        characters[index].SetActive(true);
    }

    public void SwitchNext()
    {
        characters[index].SetActive(false);

        index++;
        if (index == characters.Length)
            index = 0;

        characters[index].SetActive(true);
    }

    public void SelectPlayer()
    {
        PlayerPrefs.SetInt("selectedHero", index);
        Debug.Log(PlayerPrefs.GetInt("selectedHero"));
        PlayerPrefs.SetInt("isPlayerSelectedHero", 1);
        SceneManager.LoadScene("Tutorial");
    }
}
