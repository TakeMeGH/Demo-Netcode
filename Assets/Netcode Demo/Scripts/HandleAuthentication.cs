using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

public class HandleAuthentication : MonoBehaviour
{

    string playerName;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async void Authenticate(string playerName) {
        this.playerName = playerName;
        InitializationOptions initializationOptions = new InitializationOptions();
        initializationOptions.SetProfile(playerName);

        await UnityServices.InitializeAsync(initializationOptions);

        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed in! " + AuthenticationService.Instance.PlayerId);
            Debug.Log("Nama :  " + AuthenticationService.Instance.Profile);
            SceneHandler.Instance.loadNextLevel();
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }
}
