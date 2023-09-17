using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Networking.Transport.Relay;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.SceneManagement;
using TMPro;
public class LobbyListUI : MonoBehaviour
{

    [SerializeField] GameObject buttonTemp;
    [SerializeField] GameObject gridContent;
    float queryLobbyTimer = 13f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        queryLobbyTimer -= Time.deltaTime;
        if(queryLobbyTimer < 0){
            queryLobbyTimer = 13f;
            UpdateLobby();
        }
    }

    async void UpdateLobby(){
        QueryResponse lobbies = await Lobbies.Instance.QueryLobbiesAsync();
        int cnt = gridContent.transform.childCount;
        for(int i = cnt - 1; i > 0; i--){
            Destroy(gridContent.transform.GetChild(i));
        }

        foreach(Lobby lobby in lobbies.Results){
            GameObject createdButton = Instantiate(buttonTemp, gridContent.transform);
            createdButton.GetComponent<LobbyData>().lobbyId = lobby.Id;
            createdButton.GetComponent<LobbyData>().lobbyId = lobby.Name;
            createdButton.SetActive(true);
        }
    }
}
