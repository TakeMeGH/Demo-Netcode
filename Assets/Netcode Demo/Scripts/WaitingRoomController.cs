using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;
using TMPro;

public class WaitingRoomController : MonoBehaviour
{
    Lobby joinedLobby;
    [SerializeField] TMP_Text playerCount;
    [SerializeField] TMP_Text lobbyName;
    [SerializeField] TMP_Text lobbyCode;
    float refreshTimer = 10f;
    void Start()
    {
        joinedLobby = GameObject.FindGameObjectWithTag("LobbyController").GetComponent<LobbyController>().joinedLobby;
    }

    // Update is called once per frame
    void Update()
    {
        joinedLobby = GameObject.FindGameObjectWithTag("LobbyController").GetComponent<LobbyController>().joinedLobby;
        refreshTimer -= Time.deltaTime;
        if(refreshTimer < 0 && joinedLobby != null){
            refreshTimer = 10f;
            UpdateUI();
        }
    }

    void UpdateUI(){
        playerCount.text = joinedLobby.Players.Count.ToString() + " / " + joinedLobby.MaxPlayers;
        lobbyName.text = joinedLobby.Name;
        lobbyCode.text = joinedLobby.LobbyCode;
    }
}
