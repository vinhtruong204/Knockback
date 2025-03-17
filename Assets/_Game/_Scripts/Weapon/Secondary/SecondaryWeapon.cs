using UnityEngine;

public class SecondaryWeapon : PrimaryWeapon
{
    public SecondaryWeapon(string name, int maxAmmo, float fireRate, float reloadTime, float weight, GameObject model)
        : base(name, maxAmmo, fireRate, reloadTime, weight, model)
    {
        Type = WeaponType.Secondary;
    }
}
