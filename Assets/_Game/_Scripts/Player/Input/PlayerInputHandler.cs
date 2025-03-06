using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction moveInputAction;
    private InputAction jumpInputAction;
    private InputAction shootInputAction;
    private InputAction throwBombInputAction;

    // Events
    public static event Action<Vector2> OnMove;
    public static event Action OnJump;
    public static event Action OnShoot;
    public static event Action OnThrowBomb;

    private float time;

    private void OnEnable()
    {
        // Get input action
        playerInput = GetComponent<PlayerInput>();

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
        OnThrowBomb?.Invoke();
    }

    private void OnShootInput(InputAction.CallbackContext context)
    {
        OnShoot?.Invoke();
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
