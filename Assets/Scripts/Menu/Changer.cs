using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class Changer : MonoBehaviour
{
    public Action onChangeParametrs;
    [SerializeField] private TMP_InputField _adressInput;
    [SerializeField] private Dropdown _dropdown;
    [SerializeField] private PlayerNetwork _playerNetwork;

    public void SetColor()
    {
        Color color;
        switch (_dropdown.value)
        {
            case 0: color = Color.black; break;
            case 1: color = Color.red; break;
            case 2: color = Color.blue; break;
            case 3: color = Color.green; break;
            case 4: color = GetRandomColor(); break;
            default: color = Color.white; break;
        }
        _playerNetwork.CmdSetNewColor(color);
        onChangeParametrs?.Invoke();
    }
    public void SetNickName()
    {
        _playerNetwork.CmdSetNewnickName(_adressInput.text);
        onChangeParametrs?.Invoke();
    }

    public void SetPlayer()
    {
        List<PlayerNetwork> players = ((PLNetworkManager)NetworkManager.singleton).players;
        foreach (PlayerNetwork p in players)
        {
            if (p.hasAuthority)
            {
                _playerNetwork = p;
            }
        }
    }
    private Color GetRandomColor()
    {
        return new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
    }

}
