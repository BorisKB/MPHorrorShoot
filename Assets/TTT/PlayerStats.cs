using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using playerSystem;

public class PlayerStats : NetworkBehaviour, IDamagable
{
    public Action<int> OnHealthChanged;
    public Action<GameObject> OnDied;
    [SerializeField] private Rigidbody[] ragdolls;
    [SerializeField] private int _health;
    private bool _isDeath = false;
    void Start()
    {
        if (ragdolls.Length != 0)
        {
            for (int i = 0; i < ragdolls.Length; i++)
            {
                ragdolls[i].isKinematic = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (hasAuthority)
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                if (isServer)
                {
                    RpcDeath();
                }
                else
                {
                    CmdDeath();
                }
                
            }
        }
    }
    public void TakeDamage(int amount)
    {
        if (_isDeath == false)
        {
            if (isServer)
            {
                RpcTakeDamage(amount);
            }
            else
            {
                CmdTakeDamage(amount);
            }
        }
    }
    [Command(requiresAuthority = false)]
    private void CmdTakeDamage(int amount)
    {
        RpcTakeDamage(amount);
    }
    [ClientRpc]
    private void RpcTakeDamage(int amount)
    {
        _health = Mathf.Max(_health - amount, 0);
        OnHealthChanged?.Invoke(_health);
        if (_health <= 0)
        {
            if (isServer)
            {
                RpcDeath();
            }
            else
            {
                CmdDeath();
            }
        }
    }
    [Command]
    private void CmdDeath()
    {
        RpcDeath();
    }

    [ClientRpc]
    private void RpcDeath()
    {
        _isDeath = true;
        if (ragdolls.Length != 0)
        {
            for (int i = 0; i < ragdolls.Length; i++)
            {
                ragdolls[i].isKinematic = false;
            }
        }
        if (TryGetComponent(out ThirdPersonController player))
        {
            player.enabled = false;
            GetComponent<ThirdPersonShooter>().enabled = false;
            GetComponent<Animator>().enabled = false;
        }
        else
        {
            GetComponent<Enemy>().OnDeathEnemy();
            GetComponent<Enemy>().enabled = false;
            GetComponent<Animator>().SetTrigger("Death");
        }

        OnDied?.Invoke(gameObject);
    }

}
