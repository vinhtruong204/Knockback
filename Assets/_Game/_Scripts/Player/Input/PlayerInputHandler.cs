using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInput playerInput;

    // Move event
    public static event Action<Vector2> OnMove;
    private InputAction moveInputAction;

    private void OnEnable()
    {
        // Get input action
        playerInput = GetComponent<PlayerInput>();
        moveInputAction = playerInput.actions["Move"];

        if (moveInputAction == null)
        {
            Debug.LogError("Move input action not found!");
            return;
        }

        // Subscribe to input events
        moveInputAction.performed += OnMoveInput;
        moveInputAction.canceled += OnMoveInput;
    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        OnMove?.Invoke(context.ReadValue<Vector2>());
    }

    private void OnDisable()
    {
        if (moveInputAction == null) return;

        moveInputAction.performed -= OnMoveInput;
        moveInputAction.canceled -= OnMoveInput;
    }
}
