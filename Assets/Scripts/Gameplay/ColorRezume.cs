using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorRezume : MonoBehaviour
{
    [SerializeField] private Material[] materials;
    void Start()
    {
        List<PlayerNetwork> players = ((PLNetworkManager)NetworkManager.singleton).players;

        for (int i = 0; i < players.Count; i++)
        {
            materials[i].color = players[i].GetTeamColor();
        }

    }

    public Material GetPlayerMat(int index)
    {
        return materials[index];
    }
}
