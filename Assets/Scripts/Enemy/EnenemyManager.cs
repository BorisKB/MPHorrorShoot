using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using playerSystem;
using System.Threading;

public class EnenemyManager : NetworkBehaviour
{
    [SerializeField] private GameObject _enemtPrefab;
    [SerializeField] private Transform _posSpawn;
    [SerializeField] private List<Enemy> _enemyList = new List<Enemy>();
    [SerializeField] private List<Transform> _PlayersList = new List<Transform>();
    [SerializeField] private bool isSpawning = false;

    private void Start()
    {
        if (isServer)
        {
            PLNetworkManager.OnSpawnedPlayerHero += HeroSpawned;
            StartCoroutine(SDa());
            if(_PlayersList.Count == 0)
            {
                ThirdPersonController[] players = FindObjectsOfType<ThirdPersonController>();
                for (int i = 0; i < players.Length; i++)
                {
                    HeroSpawned(players[i]);
                }
            }
        }
    }
    private void Update()
    {
        if (isServer)
        {
            if (!isSpawning)
            {
                isSpawning = true;
                StartCoroutine(SDa());
            }
        }
    }
    private IEnumerator SDa()
    {
        yield return new WaitForSeconds(Random.Range(5, 60));
        isSpawning = false;
        CmdSpawnMonster();
    }
    [Command(requiresAuthority = false)]
    private void CmdSpawnMonster()
    {
        GameObject monster = Instantiate(_enemtPrefab, _posSpawn.position, Quaternion.identity);
        NetworkServer.Spawn(monster, connectionToServer);
        _enemyList.Add(monster.GetComponent<Enemy>());
        monster.GetComponent<PlayerStats>().OnDied += EnemyDied;
        for (int i = 0; i < _PlayersList.Count; i++)
        {
            monster.GetComponent<Enemy>().AddTarget(_PlayersList[i].transform);
        }
    }

    private void EnemyDied(GameObject enemy)
    {
        enemy.GetComponent<PlayerStats>().OnDied -= EnemyDied;
        _enemyList.Remove(enemy.GetComponent<Enemy>());
        StartCoroutine(DestroyWithTimer(enemy));
    }
    private IEnumerator DestroyWithTimer(GameObject enemy)
    {
        yield return new WaitForSeconds(10f);
        NetworkServer.Destroy(enemy);
    }
    private void OnDestroy()
    {
        if (hasAuthority)
        {
            PLNetworkManager.OnSpawnedPlayerHero -= HeroSpawned;
        }
    }

    private void HeroSpawned(ThirdPersonController playerHero)
    {
        _PlayersList.Add(playerHero.transform);
        playerHero.GetComponent<PlayerStats>().OnDied += HeroDied;
        if (_enemyList.Count > 0)
        {
            foreach (var item in _enemyList)
            {
                item.AddTarget(playerHero.transform);
            }
        }
    }
    private void HeroDied(GameObject gameObject)
    {
        gameObject.GetComponent<PlayerStats>().OnDied -= HeroDied;
        _PlayersList.Remove(gameObject.transform);
        if(_enemyList.Count > 0)
        {
            foreach (var item in _enemyList)
            {
                item.RemoveTarget(gameObject.transform);
            }
        }
    }
}
