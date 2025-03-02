using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction moveInputAction;
    private InputAction jumpInputAction;

    // Move event
    public static event Action<Vector2> OnMove;
    public static event Action OnJump;

    private void OnEnable()
    {
        // Get input action
        playerInput = GetComponent<PlayerInput>();
        moveInputAction = playerInput.actions["Move"];
        jumpInputAction = playerInput.actions["Jump"];

        // Check if the input action is null
        if (jumpInputAction != null)
        {
            // Subscribe to input events
            jumpInputAction.performed += OnJumpInput;
        }
        else
        {
            Debug.LogError("Jump input action not found!");
        }



        if (moveInputAction != null)
        {
            // Subscribe to input events
            moveInputAction.performed += OnMoveInput;
            moveInputAction.canceled += OnMoveInput;
        }
        else
        {
            Debug.LogError("Move input action not found!");
        }

    }

    private void OnJumpInput(InputAction.CallbackContext context)
    {
        OnJump?.Invoke();
    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        OnMove?.Invoke(context.ReadValue<Vector2>());
    }

    private void OnDisable()
    {
        // Unsubscribe from input events
        if (jumpInputAction != null)
        {
            jumpInputAction.performed -= OnJumpInput;
        }


        if (moveInputAction != null)
        {
            moveInputAction.performed -= OnMoveInput;
            moveInputAction.canceled -= OnMoveInput;
        }
    }
}
