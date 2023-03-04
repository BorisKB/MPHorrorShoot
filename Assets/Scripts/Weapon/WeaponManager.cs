using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using weaponSystem;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField] private Weapon _cureentWeapon;
    [SerializeField] private Weapon[] _weapons;

    private void Start()
    {
        if (hasAuthority)
        {
            _cureentWeapon.OnHitPlayerBox += DamagePlayer;
            _cureentWeapon.OnHitInSomething += CmdSpawnParticle;
        }
    }
    public Weapon GetCurrentWeapon()
    {
        return _cureentWeapon;
    }
    [Command]
    public void CmdSwapWeapon(int i)
    {
        RpcSwapWeapon(i);
    }
    [ClientRpc]
    public void RpcSwapWeapon(int i)
    {
        _cureentWeapon.OnHitPlayerBox -= DamagePlayer;
        _cureentWeapon.OnHitInSomething -= CmdSpawnParticle;
        _cureentWeapon.gameObject.SetActive(false);
        _cureentWeapon = _weapons[i];
        _cureentWeapon.gameObject.SetActive(true);
        _cureentWeapon.OnHitPlayerBox += DamagePlayer;
        _cureentWeapon.OnHitInSomething += CmdSpawnParticle;
    }

    public void Shoot()
    {
        _cureentWeapon.Fire();
    }
    public void Reload()
    {
        _cureentWeapon.Reload();
    }
    private void DamagePlayer(PlayerHitBoxes playerHitBoxes, int amount)
    {
        if (hasAuthority)
        {
            if (isServer)
            {
                RpcDamagePlayer(playerHitBoxes.gameObject, amount);
            }
            else
            {
                CmdDamagePlayer(playerHitBoxes.gameObject, amount);
            }
        }
    }
    [Command]
    private void CmdDamagePlayer(GameObject playerHitBoxes, int amount)
    {
        RpcDamagePlayer(playerHitBoxes, amount);
    }
    [ClientRpc]
    private void RpcDamagePlayer(GameObject playerHitBoxes, int amount)
    {
        playerHitBoxes.GetComponent<PlayerHitBoxes>().TakeDamagePlayer(amount);
    }
    [Command]
    public void CmdShoot(Vector3[] hit)
    {
        RPCShoot(hit);
    }
    [ClientRpc]
    public void RPCShoot(Vector3[] hit)
    {
        CmdSpawnParticle(hit);
    }
        
    [Command]
    private void CmdSpawnParticle(Vector3[] hit)
    {
        _cureentWeapon.SpawnParticle(hit, connectionToServer);
        RpcPlayParticle();
    }
    [ClientRpc]
    private void RpcPlayParticle()
    {
        _cureentWeapon.PlayMuzzleFlashVfx();
    }
}
