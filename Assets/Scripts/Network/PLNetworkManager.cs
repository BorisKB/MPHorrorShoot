using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Mirror;
using System;
using playerSystem;

public class PLNetworkManager : NetworkManager
{
    [SerializeField] private ThirdPersonController somePlayer;
    
    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;
    public static event Action<ThirdPersonController> OnSpawnedPlayerHero;

    private bool isGameInProgress = false;

    public List<PlayerNetwork> players { get; } = new List<PlayerNetwork>();

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        if (isGameInProgress) 
        {
            conn.Disconnect();
        }
    }

    public override void OnStopServer()
    {
        players.Clear();

        isGameInProgress = false;
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        PlayerNetwork player = conn.identity.GetComponent<PlayerNetwork>();

        players.Remove(player);

        base.OnServerDisconnect(conn);
    }
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        PlayerNetwork player = conn.identity.GetComponent<PlayerNetwork>();

        players.Add(player);

        player.SetDisplayName($"Player {players.Count}");

        player.SetTeamColor(new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f)));

        player.SetIsParrtyOwner(players.Count == 1);

        DontDestroyOnLoad(player);
    }
    public override void OnClientConnect()
    {
        base.OnClientConnect();

        ClientOnConnected?.Invoke();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        ClientOnDisconnected?.Invoke();
    }

    public override void OnServerSceneChanged(string newSceneName)
    {
        base.OnServerSceneChanged(newSceneName);
        if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
        {
            foreach (PlayerNetwork player in players)
            {
                SpawnHero(player.connectionToClient);
            }
        }
    }
    private void OnPlayerDied(GameObject conn)
    {
        StartCoroutine(TimeToRespawn(conn));
    }
    private IEnumerator TimeToRespawn(GameObject conn)
    {
        conn.GetComponent<PlayerStats>().OnDied -= OnPlayerDied;
        yield return new WaitForSeconds(15f);
        SpawnHero(conn.GetComponent<NetworkIdentity>().connectionToClient);
        NetworkServer.Destroy(conn);
    }
    private void SpawnHero(NetworkConnectionToClient conn)
    {
        ThirdPersonController playerInnstance1 = Instantiate(somePlayer, startPositions[UnityEngine.Random.Range(0, startPositions.Count)].position, Quaternion.identity);
        NetworkServer.Spawn(playerInnstance1.gameObject, conn);
        playerInnstance1.GetComponent<PlayerStats>().OnDied += OnPlayerDied;
        OnSpawnedPlayerHero?.Invoke(playerInnstance1);
    }

    public override void OnStopClient()
    {
        players.Clear();
    }
    public void StartGame(int numberMap) 
    {
        if(players.Count >= 1)
        {
            isGameInProgress = true;

            ServerChangeScene($"Scene_Map_{numberMap}");
        }
    }
}
