using System;
using UnityEngine;

public class FlipPlayer : MonoBehaviour
{
    private void OnEnable()
    {
        // Subscribe to input events
        PlayerInputHandler.OnMove += OnMoveInput;
    }

    private void OnMoveInput(Vector2 moveInput)
    {
        if (moveInput.x != 0f)
            UpdatePlayerOrientation(moveInput);
    }

    /// <summary>
    /// Flip the player sprite based on the move direction
    /// </summary>
    private void UpdatePlayerOrientation(Vector2 moveInput)
    {
        if (moveInput.x > 0)
            transform.parent.localScale = new Vector3(1, 1, 1);
        else if (moveInput.x < 0)
            transform.parent.localScale = new Vector3(-1, 1, 1);
    }

    private void OnDisable()
    {
        // Unsubscribe from input events
        PlayerInputHandler.OnMove -= OnMoveInput;
    }
}
