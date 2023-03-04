using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    [SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))]
    private bool isPartyOwner = false;
    [SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))]
    private string displayName = "player";
    [SyncVar(hook = nameof(HandleColorUpdated))]
    public Color teamColor = Color.black;

    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;
    public static event Action ClientOnInfoUpdated;

    public Color GetTeamColor()
    {
        return teamColor;
    }
    [Server]
    public void SetTeamColor(Color color)
    {
        teamColor = color;
    }
    public string GetDisplayName() 
    {
        return displayName;
    }

    [Server]
    public void SetDisplayName(string newDisplayName)
    {
        displayName = newDisplayName;
    }
    public bool GetIsPartyOwner()
    {
        return isPartyOwner;
    }
    [Server]
    public void SetIsParrtyOwner(bool state)
    {
        isPartyOwner = state;
    }

    [Command]
    public void CmdStartGame(int numberMap) 
    {
        if (isPartyOwner)
        {
            ((PLNetworkManager)NetworkManager.singleton).StartGame(numberMap);
        }
    }
    [Command]
    public void CmdSetNewColor(Color color)
    {
        SetTeamColor(color);
    }
    [Command]
    public void CmdSetNewnickName(string name)
    {
        SetDisplayName(name);
    }
    public override void OnStartClient()
    {
        if (!NetworkServer.active)
        {
            ((PLNetworkManager)NetworkManager.singleton).players.Add(this);
            DontDestroyOnLoad(gameObject);
        }
    }
    public override void OnStopClient()
    {
        ClientOnInfoUpdated?.Invoke();

        if (isClientOnly) 
        {
            ((PLNetworkManager)NetworkManager.singleton).players.Remove(this);
        }
    }

    private void ClientHandleDisplayNameUpdated(string oldDisplayName, string newDisplayName)
    {
        ClientOnInfoUpdated?.Invoke();
    }
    private void AuthorityHandlePartyOwnerStateUpdated(bool oldState, bool newState) 
    {
        if (hasAuthority)
        {
            AuthorityOnPartyOwnerStateUpdated?.Invoke(newState);
        }
    }
    private void HandleColorUpdated(Color oldColor, Color newColor)
    {
        teamColor = newColor;
        ClientOnInfoUpdated?.Invoke();
    }
}
