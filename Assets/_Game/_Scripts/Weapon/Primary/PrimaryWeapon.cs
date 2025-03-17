using System.Collections;
using UnityEngine;

public class PrimaryWeapon : WeaponBase, IFireable, IReloadable
{
    public int Ammo { get; private set; }
    public int MaxAmmo { get; private set; }
    public float FireRate { get; private set; }
    public float ReloadTime { get; private set; }

    private float lastFireTime;

    private async void Awake()
    {
        MaxAmmo = 30;
        Ammo = MaxAmmo;
        FireRate = 5f;
        ReloadTime = 2.0f;
        lastFireTime = -FireRate;

        var (asset, handle) = await AddressableLoader<WeaponData>.LoadAssetAsync("AK47_Basic");

        Init(asset);

        AddressableLoader<WeaponData>.ReleaseHandle(handle);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) Attack();
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
                Ammo -= 10;
                lastFireTime = Time.time;
                Debug.Log(Name + " fires! Ammo left: " + Ammo);
                if (Ammo == 0) Reload();
            }
            else
            {
                Debug.Log(Name + " is out of ammo!");
            }
        }
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
