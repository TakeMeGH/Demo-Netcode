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




public class LobbyController : MonoBehaviour
{

    public Lobby joinedLobby;
    bool isOwner = false;
    public List<GameObject> lobbyUIlist;
    float heartbeatTimer = 10f;
    float joinRelayTimer = 10f;
    float lobbyPollingTimer = 10f;

    [SerializeField] Button createLobbyButton;
    [SerializeField] Button quickJoinButton;
    [SerializeField] Button startButton;
    [SerializeField] Button joinRoomButton;
    [SerializeField] TMP_InputField lobbyCodeInput;

    [SerializeField] Canvas joinUI;
    [SerializeField] Canvas waitingUI;
    bool isJoin = false;


    private void Awake()
    {
        createLobbyButton.onClick.AddListener(() =>
        {
            CreateLobby();
        });
        quickJoinButton.onClick.AddListener(() =>
        {
            QuickJoinLobby();
        });

        startButton.onClick.AddListener(() =>
        {
            StartGame();
        });
        
        joinRoomButton.onClick.AddListener(() => {
            JoinLobby(lobbyCodeInput.text);
        });
    }
    // Update is called once per frame
    void Update()
    {
        HandleLobbyHeartbeat();
        LobbyPolling();

        joinRelayTimer -= Time.deltaTime;
        if (joinRelayTimer < 0)
        {
            joinRelayTimer = 10f;
            checkRelay();
        }
    }

    async void LobbyPolling(){
        if(joinedLobby != null){
            lobbyPollingTimer -= Time.deltaTime;
            if(lobbyPollingTimer < 0){
                lobbyPollingTimer = 10f;
                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                joinedLobby = lobby;
            }
        }
    }

    public async void CreateLobby()
    {
        try
        {
            Debug.Log("crete lobby");
            string lobbyName = "Test Lobby";
            int maxPlayers = 4;
            CreateLobbyOptions options = new CreateLobbyOptions();

            options.Player = new Player(
                id: AuthenticationService.Instance.PlayerId,
                data: new Dictionary<string, PlayerDataObject>()
                {
                    {
                        "Nama", new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Member, // Visible only to members of the lobby.
                            value: AuthenticationService.Instance.Profile)
                    }
                });
            options.Data = new Dictionary<string, DataObject>() {
                     { "KEY_RELAY_JOIN_CODE" , new DataObject(DataObject.VisibilityOptions.Member, "nokey") }};
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            joinedLobby = lobby;
            Debug.Log("Lobby berhasil dibuat : " + lobbyName + " " + maxPlayers);
            isOwner = true;
            joinUI.enabled = false;
            waitingUI.enabled = true;
            Debug.Log(waitingUI.enabled);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void HandleLobbyHeartbeat()
    {
        if (joinedLobby != null && isOwner)
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0f)
            {
                float heartbeatTimerMax = 10f;
                heartbeatTimer = heartbeatTimerMax;

                Debug.Log("Heartbeat");
                await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }

    public async void JoinLobby(string gameCode)
    {
        try
        {
            Debug.Log(lobbyCodeInput.text == "AJAKD");
            Debug.Log(gameCode);
            JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions();
            options.Player = new Player(
                id: AuthenticationService.Instance.PlayerId,
                data: new Dictionary<string, PlayerDataObject>()
                {
                    {
                        "Nama", new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Member, // Visible only to members of the lobby.
                            value: AuthenticationService.Instance.Profile)
                    }
                });

            Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(gameCode, options);
            joinedLobby = lobby;

            joinUI.enabled = false;
            waitingUI.enabled = true;

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void JoinLobbyById(string gameCode)
    {
        try
        {
            Debug.Log(lobbyCodeInput.text == "AJAKD");
            Debug.Log(gameCode);
            JoinLobbyByIdOptions options = new JoinLobbyByIdOptions();
            options.Player = new Player(
                id: AuthenticationService.Instance.PlayerId,
                data: new Dictionary<string, PlayerDataObject>()
                {
                    {
                        "Nama", new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Member, // Visible only to members of the lobby.
                            value: AuthenticationService.Instance.Profile)
                    }
                });

            Lobby lobby = await Lobbies.Instance.JoinLobbyByIdAsync(gameCode, options);
            joinedLobby = lobby;

            joinUI.enabled = false;
            waitingUI.enabled = true;

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void QuickJoinLobby()
    {
        try
        {
            Debug.Log("Quick Join");
            QuickJoinLobbyOptions options = new QuickJoinLobbyOptions();

            options.Player = new Player(
                id: AuthenticationService.Instance.PlayerId,
                data: new Dictionary<string, PlayerDataObject>()
                {
                    {
                        "Nama", new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Member, // Visible only to members of the lobby.
                            value: AuthenticationService.Instance.Profile)
                    }
                });

            Lobby lobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);
            joinedLobby = lobby;

            Debug.Log("Nama lobby : " + joinedLobby.Name);
            foreach (Player player in joinedLobby.Players)
            {
                Debug.Log("Player ID : " + player.Data["Nama"].Value);
            }

            joinUI.enabled = false;
            waitingUI.enabled = true;
            // OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    void StartGame()
    {
        CreateRelay();
    }

    public async void CreateRelay()
    {
        try
        {
            isJoin = true;
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4);

            string gameCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject> {
                     { "KEY_RELAY_JOIN_CODE" , new DataObject(DataObject.VisibilityOptions.Member, gameCode) }
                 }
            });
            Debug.Log(gameCode);
            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.LoadScene("GameMenu", LoadSceneMode.Single);

        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void JoinRelay(string gameCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(gameCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            isJoin = true;

            NetworkManager.Singleton.StartClient();

        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    void checkRelay()
    {
        if(isJoin) return;
        if(joinedLobby == null) return;
        Debug.Log("check relay");
        if (joinedLobby != null && joinedLobby.Data["KEY_RELAY_JOIN_CODE"].Value != "nokey")
        {
            Debug.Log(joinedLobby.Data["KEY_RELAY_JOIN_CODE"].Value);
            JoinRelay(joinedLobby.Data["KEY_RELAY_JOIN_CODE"].Value);
        }

    }


    // public async void debugListLobby()
    // {
    //     try
    //     {
    //         QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
    //         Debug.Log("Banyak lobby : " + queryResponse.Results.Count);
    //         foreach (Lobby lobby in queryResponse.Results)
    //         {
    //             Debug.Log(lobby.Name + " " + lobby.MaxPlayers);
    //         }
    //     }
    //     catch (LobbyServiceException e)
    //     {
    //         Debug.Log(e);
    //     }
    // }

    // public async void debugCurrentLobby()
    // {
    //     Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
    //     joinedLobby = lobby;
    //     Debug.Log("Nama lobby : " + joinedLobby.Name);
    //     foreach (Player player in joinedLobby.Players)
    //     {
    //         Debug.Log("Player ID : " + player.Id);
    //     }
    // }

    // public async void showLobby()
    // {
    //     if (showLobbyTimer < 0)
    //     {
    //         showLobbyTimer = 10f;
    //         Debug.Log("SHOW LOBBY");
    //         QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

    //         int index = 0;
    //         foreach (Lobby lobby in queryResponse.Results)
    //         {
    //             if (index >= 3) break;
    //             lobbyUIlist[index].GetComponent<LobbyUIController>().gameCode = lobby.Id;
    //             lobbyUIlist[index].GetComponent<LobbyUIController>().changeText(lobby.Name);
    //             Debug.Log("Lobby Code Show : " + lobby.Id + " " + lobby.Name);
    //         }
    //     }
    //     else showLobbyTimer -= Time.deltaTime;
    // }
}
