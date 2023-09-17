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
    float refreshTimer = 3f;
    // Start is called before the first frame update
    void Start()
    {
        joinedLobby = GameObject.FindGameObjectWithTag("LobbyController").GetComponent<LobbyController>().joinedLobby;
    }

    // Update is called once per frame
    void Update()
    {
        if(joinedLobby == null){
            joinedLobby = GameObject.FindGameObjectWithTag("LobbyController").GetComponent<LobbyController>().joinedLobby;
        }
        if(refreshTimer < 0 && joinedLobby != null){
            refreshTimer = 3f;
            UpdateUI();
        }
        refreshTimer -= Time.deltaTime;
    }

    async void UpdateUI(){
        Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
        joinedLobby = lobby;

        playerCount.text = joinedLobby.Players.Count.ToString() + " / " + joinedLobby.MaxPlayers;
        lobbyName.text = joinedLobby.Name;
        lobbyCode.text = joinedLobby.LobbyCode;
    }
}
