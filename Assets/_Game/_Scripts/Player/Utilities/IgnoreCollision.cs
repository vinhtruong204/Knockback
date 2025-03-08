using System;
using System.Collections;
using UnityEngine;

public class IgnoreCollision : MonoBehaviour
{
    private PlayerInputHandler playerInputHandler;

    private void Start()
    {
        playerInputHandler = transform.parent.GetComponentInChildren<PlayerInputHandler>();
        
        // Subscribe to input events
        playerInputHandler.OnMove += OnMoveInput;
    }

    private void OnMoveInput(Vector2 vector)
    {
        if (vector.y < 0f)
        {
            // Ignore collision with ground
            StartCoroutine(IgnoreCollisionWithGround());
        }
    }

    /// <summary>
    /// Ignore collision with ground for a short duration
    /// </summary>
    /// <returns></returns>
    private IEnumerator IgnoreCollisionWithGround()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Ground"), true);
        yield return new WaitForSeconds(0.6f);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Ground"), false);
    }

    private void OnDisable()
    {
        // Unsubscribe from input events
        playerInputHandler.OnMove -= OnMoveInput;
    }
}
