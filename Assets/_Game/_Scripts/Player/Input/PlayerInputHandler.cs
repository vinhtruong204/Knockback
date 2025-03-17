using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputHandler : NetworkBehaviour
{
    private PlayerInput playerInput;
    private InputAction moveInputAction;
    private InputAction jumpInputAction;
    private InputAction attackInputAction;
    private InputAction throwInputAction;

    // Events
    public event Action<Vector2> OnMove;
    public event Action OnJump;
    public event Action OnAttack;
    public event Action OnThrow;

    private float time;

    public override void OnNetworkSpawn()
    {
        playerInput = GetComponent<PlayerInput>();

        // Check if the player is the owner of this object
        if (!IsOwner)
        {
            // Disable this script if not the owner
            this.enabled = false;
            playerInput.enabled = false;
            return;
        }

        InitialMoveInputAction();

        InitialJumpInputAction();

        InitialAttackInputAction();

        InitialThrowBombInputAction();
    }

    private void InitialMoveInputAction()
    {
        moveInputAction = playerInput.actions["Move"];

        // Check if the input action is null
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

    private void InitialJumpInputAction()
    {
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
    }

    private void InitialAttackInputAction()
    {
        attackInputAction = playerInput.actions["Attack"];

        // Check if the input action is null
        if (attackInputAction != null)
        {
            // Subscribe to input events
            attackInputAction.performed += OnAttackInput;
        }
        else
        {
            Debug.LogError("Attack input action not found!");
        }
    }

    private void InitialThrowBombInputAction()
    {
        throwInputAction = playerInput.actions["Throw"];

        // Check if the input action is null
        if (throwInputAction != null)
        {
            // Subscribe to input events
            throwInputAction.started += OnThrowInputStarted;
            throwInputAction.performed += OnThrowInputPerformed;
            throwInputAction.canceled += OnThrowInputCanceled;
        }
        else
        {
            Debug.LogError("Throw input action not found!");
        }
    }

    private void OnThrowInputCanceled(InputAction.CallbackContext context)
    {
        // TO DO: Send duration of the throw bomb input

    }

    private void OnThrowInputStarted(InputAction.CallbackContext context)
    {
        time = Time.time;
    }

    private void OnThrowInputPerformed(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;
        OnThrow?.Invoke();
    }

    private void OnAttackInput(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;
        OnAttack?.Invoke();
    }

    private void OnJumpInput(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;
        OnJump?.Invoke();
    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;
        OnMove?.Invoke(context.ReadValue<Vector2>());
    }

    private void OnDisable()
    {
        if (!IsOwner) return;

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

        if (attackInputAction != null)
        {
            attackInputAction.performed -= OnAttackInput;
        }

        if (throwInputAction != null)
        {
            throwInputAction.started -= OnThrowInputStarted;
            throwInputAction.performed -= OnThrowInputPerformed;
            throwInputAction.canceled -= OnThrowInputCanceled;
        }
    }
}
