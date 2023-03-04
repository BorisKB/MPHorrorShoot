using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject lobbyUI;
    [SerializeField] private Button startGameButton;
    [SerializeField] private PlayerDisplay[] playerDisplays;
    [SerializeField] private Changer _changer;
    [SerializeField] private Dropdown _CurrentMap;

    void Start()
    {
        PLNetworkManager.ClientOnConnected += HandleClientConnected;
        PlayerNetwork.AuthorityOnPartyOwnerStateUpdated += AuthorityHandlePartyOwnerStateUpdated;
        PlayerNetwork.ClientOnInfoUpdated += ClientHandleInfoUpdated;
        _changer.onChangeParametrs += ClientHandleInfoUpdated;
    }

    private void OnDestroy()
    {
        PLNetworkManager.ClientOnConnected -= HandleClientConnected;
        PlayerNetwork.AuthorityOnPartyOwnerStateUpdated -= AuthorityHandlePartyOwnerStateUpdated;
        PlayerNetwork.ClientOnInfoUpdated -= ClientHandleInfoUpdated;
        _changer.onChangeParametrs -= ClientHandleInfoUpdated;
    }

    private void AuthorityHandlePartyOwnerStateUpdated(bool state)
    {
        startGameButton.gameObject.SetActive(state);
        _CurrentMap.gameObject.SetActive(state);
    }
    private void HandleClientConnected() 
    {
        lobbyUI.SetActive(true);
    }

    private void ClientHandleInfoUpdated() 
    {
        List<PlayerNetwork> players = ((PLNetworkManager)NetworkManager.singleton).players;

        for (int i = 0; i < players.Count; i++)
        {
            playerDisplays[i]._playerColor.gameObject.SetActive(true);
            playerDisplays[i]._playerName.text = players[i].GetDisplayName();
            playerDisplays[i]._playerColor.color = players[i].GetTeamColor();
        }

        for (int i = players.Count; i < playerDisplays.Length; i++)
        {
            playerDisplays[i]._playerName.text = "Waiting For Player...";
            playerDisplays[i]._playerColor.gameObject.SetActive(false);

        }
        _changer.SetPlayer();
    }

    public void StartGame()
    {
        NetworkClient.connection.identity.GetComponent<PlayerNetwork>().CmdStartGame(_CurrentMap.value); 
    }

    public void LeaveLobyy() 
    {
        if (NetworkServer.active && NetworkClient.isConnected) 
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();

        }
        SceneManager.LoadScene(0);

    }
}
