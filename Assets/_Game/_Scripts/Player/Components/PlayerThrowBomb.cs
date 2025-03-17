using System;
using UnityEngine;

public class PlayerThrowBomb : MonoBehaviour
{
    private PlayerInputHandler playerInputHandler;
    private void Start()
    {
        playerInputHandler = transform.parent.GetComponentInChildren<PlayerInputHandler>();
        
        playerInputHandler.OnThrow += ThrowBomb;
    }

    private void ThrowBomb()
    {
    }

    private void OnDisable()
    {
        playerInputHandler.OnThrow -= ThrowBomb;
    }
}
