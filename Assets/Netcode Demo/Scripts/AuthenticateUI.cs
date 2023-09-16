using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AuthenticateUI : MonoBehaviour {


    [SerializeField] Button authenticateButton;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] HandleAuthentication handleAuthentication;


    private void Awake() {
        authenticateButton.onClick.AddListener(() => {
            handleAuthentication.Authenticate(inputField.text);
        });
    }

}