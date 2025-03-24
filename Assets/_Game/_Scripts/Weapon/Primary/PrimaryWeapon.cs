using System;
using System.Collections;
using UnityEngine;

public class PrimaryWeapon : WeaponBase, IFireable, IReloadable
{
    public int Ammo { get; protected set; }
    public int MaxAmmo { get; protected set; }
    public int TotalAmmo { get; protected set; }
    public float FireRate { get; protected set; }
    public float ReloadTime { get; protected set; }

    public event Action<float> OnReload;
    public event Action<int, int> OnAmmoChanged;
    private float lastFireTime;

    private async void Awake()
    {
        InitializeAtrributes();

        var (asset, handle) = await AddressableLoader<WeaponData>.LoadAssetAsync("AK47_Basic");

        Init(asset);

        AddressableLoader<WeaponData>.ReleaseHandle(handle);
    }

    private void InitializeAtrributes()
    {
        MaxAmmo = 30;
        Ammo = MaxAmmo;
        TotalAmmo = 30;
        FireRate = 0.1f;
        ReloadTime = 1.0f;
        lastFireTime = -FireRate;
        Type = WeaponType.Primary;
    }

    public void ResetAmmo()
    {
        MaxAmmo = 30;
        Ammo = MaxAmmo;
        TotalAmmo = 30;

        OnAmmoChanged?.Invoke(Ammo, TotalAmmo);
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

        OnAmmoChanged?.Invoke(Ammo, TotalAmmo);

        if (Ammo == 0)
            Reload();
    }


    public void Reload()
    {
        if (!CanReload()) return;
        OnReload?.Invoke(ReloadTime);
        StartCoroutine(ReloadCoroutine());
    }

    private bool CanReload()
    {
        return TotalAmmo > 0;
    }

    private IEnumerator ReloadCoroutine()
    {
        yield return new WaitForSeconds(ReloadTime);

        if (TotalAmmo > 0)
        {
            TotalAmmo -= MaxAmmo;
            Ammo = MaxAmmo;

            // Raise event
            
            OnAmmoChanged?.Invoke(Ammo, TotalAmmo);
        }
    }
}
