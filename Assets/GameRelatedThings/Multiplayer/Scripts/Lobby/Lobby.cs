using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class Lobby : MonoBehaviourPunCallbacks
{
    public TMP_InputField createInput;
    public TMP_InputField joinInput;

    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.CreateRoom(createInput.text, roomOptions);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinInput.text);
    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom(null, 2, MatchmakingMode.FillRoom, TypedLobby.Default, null);
    }

    public void CreateRandomRoom()
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 2 }, null);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.ConnectUsingSettings();
    }


    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("MultiplayerInGame");
    }

    public virtual void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        Debug.LogError("Error: " + cause);
    }

}
