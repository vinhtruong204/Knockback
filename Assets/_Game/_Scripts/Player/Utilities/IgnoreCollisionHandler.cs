using System;
using System.Collections;
using Unity.Services.Lobbies.Models;
using UnityEngine;


/// <summary>
/// This script is attached to the player object and uses a BoxCollider2D component.\
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class IgnoreCollisionHandler : MonoBehaviour
{
    private PlayerInputHandler playerInputHandler;
    private BoxCollider2D groundCollider;
    private BoxCollider2D playerCollider;

    private void Awake()
    {
        playerInputHandler = GetComponentInChildren<PlayerInputHandler>();
        playerCollider = GetComponent<BoxCollider2D>();

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

    private IEnumerator IgnoreCollisionWithGround()
    {
        if (groundCollider != null)
        {
            // Ignore collision with the ground collider
            BoxCollider2D currentGroundCollider = groundCollider;
            
            Physics2D.IgnoreCollision(playerCollider, currentGroundCollider, true);
            yield return new WaitForSeconds(0.6f);
            Physics2D.IgnoreCollision(playerCollider, currentGroundCollider, false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collided object is on the ground layer
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            groundCollider = collision.collider as BoxCollider2D;
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from input events
        playerInputHandler.OnMove -= OnMoveInput;
    }
}
