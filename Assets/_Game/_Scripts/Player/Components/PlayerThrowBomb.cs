using System;
using UnityEngine;

public class PlayerThrowBomb : MonoBehaviour
{
    private void OnEnable()
    {
        PlayerInputHandler.OnThrowBomb += ThrowBomb;
    }

    private void ThrowBomb()
    {
        _ = ProjectilePoolManager.Instance.GetObject<Grenade>(PoolType.Grenade, transform.parent);
    }

    private void OnDisable()
    {
        PlayerInputHandler.OnThrowBomb -= ThrowBomb;
    }
}
