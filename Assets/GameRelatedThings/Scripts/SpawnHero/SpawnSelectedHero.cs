using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnSelectedHero : MonoBehaviour
{
	public int selectedPlayerHero;
	public GameObject[] selectAbleHeroes;
	public GameObject selectedPlayerHeroObject;

	void Start()
    {
		selectedPlayerHero = PlayerPrefs.GetInt("selectedHero");
		Debug.Log(selectedPlayerHero);

		if (selectAbleHeroes[selectedPlayerHero])
		{
			if(SceneManager.GetActiveScene().name == "Tutorial") { return; }
			selectedPlayerHeroObject = selectAbleHeroes[selectedPlayerHero];
			Instantiate(selectedPlayerHeroObject, transform.position, transform.rotation);
		}
	}
}
