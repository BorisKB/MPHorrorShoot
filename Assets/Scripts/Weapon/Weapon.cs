
using Mirror;
using playerSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace weaponSystem
{
    public class Weapon : MonoBehaviour
    {
        public Action<PlayerHitBoxes, int> OnHitPlayerBox;
        public Action<Vector3[]> OnHitInSomething;
        public Action<int> OnReload;
        public Action<int> OnUpdatedCurrentAmmo;
        public Action<GameObject, Transform> OnParticleSpawned;
        [SerializeField] protected LayerMask castLayer;
        [SerializeField] protected DestroyParticles _vfxHitGroundPrefab;
        [SerializeField] protected ParticlePlayer _vfxMuzzleFlash;
        [SerializeField] protected Transform _FirePoint;
        [SerializeField] protected int _damage;
        [SerializeField] protected int _currentAmmo;
        [SerializeField] protected int _maxCurrentAmmo;
        [SerializeField] protected int _allAmmo;
        [SerializeField] protected float _reloadTime;
        [SerializeField] protected float _fireRate;
        [SerializeField] protected bool _CanFire;
        protected Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);
        public virtual void Fire()
        {
            if (_CanFire)
            {
                if (_currentAmmo >= 1)
                {
                    _currentAmmo -= 1;
                    OnUpdatedCurrentAmmo?.Invoke(_currentAmmo);
                    if (_currentAmmo <= 0)
                    {
                        Reload();
                        return;
                    }
                    StartCoroutine(FireRate());
                }
            }
        }

        public void InvokeAmmoaction()
        {
            OnReload?.Invoke(_allAmmo);
            OnUpdatedCurrentAmmo?.Invoke(_currentAmmo);
        }
        private IEnumerator FireRate()
        {
            _CanFire = false;
            yield return new WaitForSeconds(_fireRate);
            _CanFire = true;
        }
        public void Reload()
        {
            if (_CanFire)
            {
                StopAllCoroutines();
                StartCoroutine(ReloadRate());
            }
        }
        private IEnumerator ReloadRate()
        {
            _CanFire = false;
            yield return new WaitForSeconds(_reloadTime);
            _CanFire = true;
            if (_currentAmmo <= 0)
            {
                _currentAmmo = Mathf.Min(_allAmmo, _maxCurrentAmmo);
                _allAmmo -= _currentAmmo;
            }
            else
            {
                if (_allAmmo > 0)
                {
                    if (_allAmmo >= _maxCurrentAmmo - _currentAmmo)
                    {
                        _allAmmo -= _maxCurrentAmmo - _currentAmmo;
                        _currentAmmo += _maxCurrentAmmo - _currentAmmo;
                    }
                    else
                    {
                        _currentAmmo += _allAmmo;
                        _allAmmo -= _allAmmo;
                    }
                }
            }
            OnReload?.Invoke(_allAmmo);
            OnUpdatedCurrentAmmo?.Invoke(_currentAmmo);
        }

        public GameObject GetVfx()
        {
            return _vfxHitGroundPrefab.gameObject;
        }

        public virtual void SpawnParticle(Vector3[] hit, NetworkConnection connection)
        {
            GameObject vfx = Instantiate(_vfxHitGroundPrefab.gameObject, hit[0], Quaternion.identity);
            vfx.transform.forward = hit[1];
            NetworkServer.Spawn(vfx, connection);
        }
        public void PlayMuzzleFlashVfx()
        {
            _vfxMuzzleFlash.PlayEffect();
        }
    }
}
