using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInputHandler playerInputHandler;
    private PlayerMovementConfigSO settings;
    private Rigidbody2D playerRigidbody;
    private Vector2 moveInput;

    private async void Awake()
    {
        await LoadPlayerSettings();

        InitializeRigidbody();
    }

    private async Task LoadPlayerSettings()
    {
        var (asset, handle) = await AddressableLoader<PlayerMovementConfigSO>.LoadAssetAsync("PlayerSettings");

        settings = asset;
        AddressableLoader<PlayerMovementConfigSO>.ReleaseHandle(handle);
    }


    private void InitializeRigidbody()
    {
        playerRigidbody = GetComponentInParent<Rigidbody2D>();
        playerRigidbody.linearDamping = settings.linearDamping;
    }

    private void Start()
    {
        playerInputHandler = transform.parent.GetComponentInChildren<PlayerInputHandler>();
        
        // Subscribe to input events
        playerInputHandler.OnMove += OnMoveInput;
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

    /// <summary>
    /// Move the player based on input and settings
    /// </summary>
    private void MovePlayer()
    {
        if (moveInput.magnitude <= 0.1f) return;

        Vector2 targetVelocity = new(moveInput.x * settings.moveSpeed, playerRigidbody.linearVelocityY);

        // Calculate the force to apply
        Vector2 force = new((targetVelocity.x - playerRigidbody.linearVelocityX) * settings.acceleration, 0);

        playerRigidbody.AddForce(force, ForceMode2D.Force);

        // Clamp the velocity to the max speed
        if (Mathf.Abs(playerRigidbody.linearVelocityX) > settings.maxSpeed)
        {
            playerRigidbody.linearVelocity = new(Mathf.Sign(playerRigidbody.linearVelocityX) * settings.maxSpeed, playerRigidbody.linearVelocityY);
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from input events
        playerInputHandler.OnMove -= OnMoveInput;
    }
}