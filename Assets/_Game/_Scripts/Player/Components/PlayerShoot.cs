using System;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    private void OnEnable()
    {
        PlayerInputHandler.OnShoot += OnShoot;
    }

    private void OnShoot()
    {
        _ = ProjectilePoolManager.Instance.GetObject<Bullet>(PoolType.Bullet, transform.parent);
    }

    private void OnDisable()
    {
        PlayerInputHandler.OnShoot -= OnShoot;
    }
}
