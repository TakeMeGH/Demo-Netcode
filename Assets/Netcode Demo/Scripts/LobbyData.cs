using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class LobbyData : MonoBehaviour
{

    public string lobbyId;
    public string lobbyName;
    Button curButton;
    // Start is called before the first frame update
    void Start()
    {
        curButton = GetComponent<Button>();
        curButton.onClick.AddListener(() => {
            JoinLobby();
        });

        transform.GetChild(0).GetComponent<TMP_Text>().text = lobbyName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void JoinLobby(){
        Debug.Log("Join by id");
        GameObject.FindGameObjectWithTag("LobbyController").GetComponent<LobbyController>().JoinLobbyById(lobbyId);
    }
}
