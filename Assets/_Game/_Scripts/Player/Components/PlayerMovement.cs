using System;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PlayerMovement : MonoBehaviour
{
    private PlayerMovementConfigSO _settings;
    private Rigidbody2D _rb;
    private Vector2 moveInput;

    private async void Awake()
    {
        // Load the player settings
        (PlayerMovementConfigSO settings, AsyncOperationHandle<PlayerMovementConfigSO> handle) = await AddressableLoader<PlayerMovementConfigSO>.LoadAssetAsync("PlayerSettings");

        _settings = settings;
        AddressableLoader<PlayerMovementConfigSO>.ReleaseHandle(handle);

        // Get the rigidbody component
        _rb = GetComponentInParent<Rigidbody2D>();
        _rb.linearDamping = _settings.linearDamping;
    }

    private void OnEnable()
    {
        // Subscribe to input events
        PlayerInputHandler.OnMove += OnMoveInput;
    }

    /// <summary>
    /// Get input from class player input
    /// </summary>
    /// <param name="vector">Move directon</param>
    private void OnMoveInput(Vector2 vector)
    {
        moveInput = vector;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        if (moveInput.magnitude <= 0.1f) return;
        
        Vector2 targetVelocity = moveInput * _settings.moveSpeed;

        // Calculate force to add 
        Vector2 force = (targetVelocity - _rb.linearVelocity) * _settings.acceleration;
        _rb.AddForce(force, ForceMode2D.Force);

        // Limit max speed
        if (_rb.linearVelocity.magnitude > _settings.maxSpeed)
        {
            _rb.linearVelocity = _rb.linearVelocity.normalized * _settings.maxSpeed;
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from input events
        PlayerInputHandler.OnMove -= OnMoveInput;
    }
}