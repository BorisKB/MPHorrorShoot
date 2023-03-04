using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIPlayerHealth : MonoBehaviour
{
    [SerializeField] private TMP_Text _currentHealth;
    [SerializeField] private Slider _healthBar;

    private PlayerStats player;
    public void Init(PlayerStats playerHealth)
    {
        player = playerHealth;
        player.OnHealthChanged += OnHealthChanged;
        player.TakeDamage(0); // пока так
    }
    public void Unsub()
    {
        player.OnHealthChanged -= OnHealthChanged;
        player = null;
    }

    public void OnHealthChanged(int currentHealth)
    {
        _currentHealth.text = currentHealth.ToString();
        _healthBar.value = currentHealth;
    }
}
