using playerSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    [SerializeField] private UIAmmoStats _ammoStats;
    [SerializeField] private UIPlayerHealth _healthStats;

    private ThirdPersonShooter _player;
    public void Init(ThirdPersonShooter player)
    {
        _player = player;
        _ammoStats.SetText(player.GetComponent<WeaponManager>().GetCurrentWeapon());
        _healthStats.Init(player.GetComponent<PlayerStats>());
        player.GetComponent<PlayerStats>().OnDied += OnPlayerDied;
    }
    private void OnPlayerDied(GameObject ss = null)
    {
        _player.GetComponent<PlayerStats>().OnDied -= OnPlayerDied;
        _ammoStats.Unsub();
        _healthStats.Unsub();
    }
}
