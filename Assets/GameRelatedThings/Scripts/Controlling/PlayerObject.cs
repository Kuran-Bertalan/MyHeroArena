using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObject : MonoBehaviour
{

    public int isPlayerSelectedHero;
    public int isPlayerFinishedWithTutorial;
    public int IsPlayerViewedCutscene;

    void Start()
    {

    }

    public int getIsPlayerViewedCutscene()
    {
        IsPlayerViewedCutscene = PlayerPrefs.GetInt("IsPlayerViewedCutscene");
        return IsPlayerViewedCutscene;
    }

    public int getIsPlayerSelectedHero()
    {
        isPlayerSelectedHero = PlayerPrefs.GetInt("isPlayerSelectedHero");
        return isPlayerSelectedHero;
    }

    public int getIsPlayerFinishedWithTutorial()
    {
        isPlayerFinishedWithTutorial = PlayerPrefs.GetInt("isPlayerFinishedWithTutorial");
        return isPlayerFinishedWithTutorial;
    }
}
