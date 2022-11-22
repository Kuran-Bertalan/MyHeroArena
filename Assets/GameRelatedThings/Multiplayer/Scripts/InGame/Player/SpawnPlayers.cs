using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    public int selectedPlayerHero;
    public GameObject[] selectAbleHeroes;
    public GameObject selectedPlayerHeroObject;
    public GameObject[] spawnPoses;

    private void Start()
    {
        Vector3 playerPosition;
        Quaternion playerRotation;
        selectedPlayerHero = PlayerPrefs.GetInt("selectedHero");
        Debug.Log(selectedPlayerHero);

        selectedPlayerHeroObject = selectAbleHeroes[selectedPlayerHero];

        if (PhotonNetwork.IsMasterClient)
        {
            playerPosition = spawnPoses[0].transform.position;
            playerRotation = Quaternion.identity;
        }
        else
        {
            playerPosition = spawnPoses[1].transform.position;
            playerRotation = Quaternion.Euler(0f, 180f, 0f);
        }
        PhotonNetwork.Instantiate(selectedPlayerHeroObject.name, playerPosition, playerRotation);
    }
}


