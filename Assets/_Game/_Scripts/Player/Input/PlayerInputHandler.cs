using System;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputHandler : NetworkBehaviour
{
    private PlayerInput playerInput;
    private InputAction moveInputAction;
    private InputAction jumpInputAction;
    private InputAction shootInputAction;
    private InputAction throwBombInputAction;

    // Events
    public event Action<Vector2> OnMove;
    public event Action OnJump;
    public event Action OnShoot;
    public event Action OnThrowBomb;

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

        InitialShootInputAction();

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

    private void InitialShootInputAction()
    {
        shootInputAction = playerInput.actions["Shoot"];

        // Check if the input action is null
        if (shootInputAction != null)
        {
            // Subscribe to input events
            shootInputAction.performed += OnShootInput;
        }
        else
        {
            Debug.LogError("Shoot input action not found!");
        }
    }

    private void InitialThrowBombInputAction()
    {
        throwBombInputAction = playerInput.actions["Throw Bomb"];

        // Check if the input action is null
        if (throwBombInputAction != null)
        {
            // Subscribe to input events
            throwBombInputAction.started += OnThrowBombInputStarted;
            throwBombInputAction.performed += OnThrowBombInputPerformed;
            throwBombInputAction.canceled += OnThrowBombInputCanceled;
        }
        else
        {
            Debug.LogError("Throw Bomb input action not found!");
        }
    }

    private void OnThrowBombInputCanceled(InputAction.CallbackContext context)
    {
        // TO DO: Send duration of the throw bomb input

    }

    private void OnThrowBombInputStarted(InputAction.CallbackContext context)
    {
        time = Time.time;
    }

    private void OnThrowBombInputPerformed(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;
        OnThrowBomb?.Invoke();
    }

    private void OnShootInput(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;
        OnShoot?.Invoke();
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

        if (shootInputAction != null)
        {
            shootInputAction.performed -= OnShootInput;
        }

        if (throwBombInputAction != null)
        {
            throwBombInputAction.started -= OnThrowBombInputStarted;
            throwBombInputAction.performed -= OnThrowBombInputPerformed;
            throwBombInputAction.canceled -= OnThrowBombInputCanceled;
        }
    }
}
