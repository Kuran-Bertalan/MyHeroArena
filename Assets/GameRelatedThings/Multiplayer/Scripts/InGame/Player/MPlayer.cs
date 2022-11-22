using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MPlayer : MonoBehaviour
{
    PhotonView view;
    Animator anim;
    public TextMeshPro nameDisplay;

    private void Start()
    {
        view = GetComponent<PhotonView>();
        anim = GetComponent<Animator>();
        Debug.Log("Player Spawn");
        if (view.IsMine)
        {
            nameDisplay.text = PhotonNetwork.NickName;
        }
        else
        {
            nameDisplay.text = view.Owner.NickName;
        }
        if (PhotonNetwork.PlayerList.Length == 2 && PhotonNetwork.IsMasterClient)
        {
            FindObjectOfType<MBattleManager>().InitTeamsToBattleManager();
        }
    }

    void Update()
    {
        if (view.IsMine)
        {
            //if (won)
            //{
            //    anim.SetBool("isWin", true);
            //}if (lost)
            //{
            //    anim.SetBool("isWin", true);
            //}
        }
    }
}
