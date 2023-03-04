using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitBoxes : MonoBehaviour
{
    [SerializeField] private float _damageMultyplyier = 1f;
    [SerializeField] private PlayerStats playerStats;
    private IDamagable _playerDamagable;

    private void Start()
    {
        _playerDamagable = playerStats.GetComponent<IDamagable>();
    }
    public void TakeDamagePlayer(int amount)
    {
        int damage = Mathf.RoundToInt(amount * _damageMultyplyier);
        _playerDamagable.TakeDamage(damage);
    }

}
