using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineInternal;

namespace weaponSystem
{
    public class WeaponRayCast : Weapon
    {
        public override void Fire()
        {
            if (_CanFire & _currentAmmo > 0)
            {
                base.Fire();
                Ray ray = Camera.main.ScreenPointToRay(center);
                if (Physics.Raycast(ray, out RaycastHit hit, 999f, castLayer))
                {
                    if (hit.collider.CompareTag("Ground"))
                    {
                        OnHitInSomething?.Invoke(new Vector3[] { hit.point, hit.normal });
                    }
                    else if (hit.collider.CompareTag("Damagable"))
                    {
                        hit.collider.GetComponent<PlayerHitBoxes>().TakeDamagePlayer(_damage);
                        OnHitInSomething?.Invoke(new Vector3[] { hit.point, hit.normal });
                    }
                }
            }
        }
        public override void SpawnParticle(Vector3[] hit, NetworkConnection connection)
        {
            GameObject vfx = Instantiate(_vfxHitGroundPrefab.gameObject, hit[0], Quaternion.identity);
            vfx.transform.forward = hit[1];
            NetworkServer.Spawn(vfx, connection);
        }
    }
}
