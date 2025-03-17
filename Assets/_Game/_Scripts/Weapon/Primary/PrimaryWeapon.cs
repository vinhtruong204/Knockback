using System.Collections;
using UnityEngine;

public class PrimaryWeapon : WeaponBase, IFireable, IReloadable
{
    public int Ammo { get; protected set; }
    public int MaxAmmo { get; protected set; }
    public float FireRate { get; protected set; }
    public float ReloadTime { get; protected set; }

    private float lastFireTime;

    private async void Awake()
    {
        MaxAmmo = 30;
        Ammo = MaxAmmo;
        FireRate = 0.5f;
        ReloadTime = 2.0f;
        lastFireTime = -FireRate;

        var (asset, handle) = await AddressableLoader<WeaponData>.LoadAssetAsync("AK47_Basic");

        Init(asset);

        AddressableLoader<WeaponData>.ReleaseHandle(handle);
    }

    public override void Attack()
    {
        Fire();
    }

    public override bool CanAttack()
    {
        return Time.time - lastFireTime >= FireRate && Ammo > 0;
    }

    public void Fire()
    {
        if (!CanAttack()) return;

        Ammo -= 1;
        lastFireTime = Time.time;

        Debug.Log($"{Name} fires! Ammo left: {Ammo}");

        if (Ammo == 0)
            Reload();
    }


    public void Reload()
    {
        StartCoroutine(ReloadCoroutine());
    }

    private IEnumerator ReloadCoroutine()
    {
        yield return new WaitForSeconds(ReloadTime);
        Ammo = MaxAmmo;
        Debug.Log(Name + " reloaded!");
    }
}
