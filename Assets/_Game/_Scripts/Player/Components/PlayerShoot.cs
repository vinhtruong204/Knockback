using System;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    private PlayerInputHandler playerInputHandler;

    private void Start()
    {
        playerInputHandler = transform.parent.GetComponentInChildren<PlayerInputHandler>();
        
        // Subscribe to input events
        playerInputHandler.OnShoot += OnShoot;
    }

    private void OnShoot()
    {
        _ = ProjectilePoolManager.Instance.GetObject<Bullet>(PoolType.Bullet, transform.parent);
    }

    private void OnDisable()
    {
        playerInputHandler.OnShoot -= OnShoot;
    }
}
