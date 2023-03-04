using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using weaponSystem;

public class UIAmmoStats : MonoBehaviour
{
    [SerializeField] private TMP_Text _currentAmmoText;
    [SerializeField] private TMP_Text _allAmmoText;

    private Weapon weapon;
    public void SetText(Weapon playerCurrentWeapon)
    {
        weapon = playerCurrentWeapon;
        weapon.OnUpdatedCurrentAmmo += UpdateCurrentAmmo;
        weapon.OnReload += UpdateAllAmmo;
        weapon.InvokeAmmoaction();
    }
    public void Unsub()
    {
        weapon.OnUpdatedCurrentAmmo -= UpdateCurrentAmmo;
        weapon.OnReload -= UpdateAllAmmo;
        weapon = null;
    }
    private void UpdateCurrentAmmo(int currentAmmo)
    {
        _currentAmmoText.text = currentAmmo.ToString();
    }
    private void UpdateAllAmmo(int allAmmo)
    {
        _allAmmoText.text = allAmmo.ToString();
    }
}
