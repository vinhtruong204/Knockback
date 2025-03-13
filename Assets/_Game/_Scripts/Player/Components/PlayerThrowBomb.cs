using System;
using UnityEngine;

public class PlayerThrowBomb : MonoBehaviour
{
    private PlayerInputHandler playerInputHandler;
    private void Start()
    {
        playerInputHandler = transform.parent.GetComponentInChildren<PlayerInputHandler>();
        
        playerInputHandler.OnThrowBomb += ThrowBomb;
    }

    private void ThrowBomb()
    {
        _ = ProjectilePoolManager.Instance.GetObject<Grenade>(PoolType.Grenade, transform.parent);
    }

    private void OnDisable()
    {
        playerInputHandler.OnThrowBomb -= ThrowBomb;
    }
}
