using UnityEngine;

public class PrimaryWeapon : WeaponBase, IFireable, IReloadable
{
    public int Ammo { get; private set; }
    public int MaxAmmo { get; private set; }
    public float FireRate { get; private set; }
    public float ReloadTime { get; private set; }

    private float lastFireTime;

    public PrimaryWeapon(string name, int maxAmmo, float fireRate, float reloadTime, float weight, GameObject model) 
        : base(name, WeaponType.Primary, weight, model)
    {
        MaxAmmo = maxAmmo;
        Ammo = maxAmmo;
        FireRate = fireRate;
        ReloadTime = reloadTime;
        lastFireTime = -fireRate;
    }

    public override void Attack()
    {
        Fire();
    }

    public void Fire()
    {
        if (Time.time - lastFireTime >= FireRate)
        {
            if (Ammo > 0)
            {
                Ammo--;
                lastFireTime = Time.time;
                Debug.Log(Name + " fires! Ammo left: " + Ammo);
            }
            else
            {
                Debug.Log(Name + " is out of ammo!");
            }
        }
    }

    public void Reload()
    {
        Ammo = MaxAmmo;
        Debug.Log(Name + " reloaded!");
    }
}
