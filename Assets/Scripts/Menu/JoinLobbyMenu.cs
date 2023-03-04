using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using System;

public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject landingPagePanel;
    [SerializeField] private TMP_InputField adressInput;
    [SerializeField] private Button joinButton;

    public void Join()
    {
        string adress = adressInput.text;

        NetworkManager.singleton.networkAddress = adress;

        NetworkManager.singleton.StartClient();

        joinButton.interactable = false;
    }

    private void OnEnable()
    {
        PLNetworkManager.ClientOnConnected += HandleClientConnected;
        PLNetworkManager.ClientOnDisconnected += HandleClientDisconnected;
    }

    private void OnDisable()
    {
        PLNetworkManager.ClientOnConnected -= HandleClientConnected;
        PLNetworkManager.ClientOnDisconnected -= HandleClientDisconnected;
    }
    private void HandleClientConnected()
    {
        joinButton.interactable = true;

        gameObject.SetActive(false);
        landingPagePanel.SetActive(false);
    }

    private void HandleClientDisconnected()
    {
        joinButton.interactable = true;

    }

}
