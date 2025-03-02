using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerMovementConfigSO _settings;
    private Rigidbody2D _rigidbody;
    private Vector2 moveInput;

    private async void Awake()
    {
        await LoadPlayerSettings();

        InitializeRigidbody();
    }

    private async Task LoadPlayerSettings()
    {
        var (asset, handle) = await AddressableLoader<PlayerMovementConfigSO>.LoadAssetAsync("PlayerSettings");

        _settings = asset;
        AddressableLoader<PlayerMovementConfigSO>.ReleaseHandle(handle);
    }


    private void InitializeRigidbody()
    {
        _rigidbody = GetComponentInParent<Rigidbody2D>();
        _rigidbody.linearDamping = _settings.linearDamping;
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

        if (moveInput.x != 0f)
            FlipPlayer();


        if (moveInput.y < 0f)
        {
            // Ignore collision with ground
            StartCoroutine(IgnoreCollisionWithGround());
        }
    }

    /// <summary>
    /// Flip the player sprite based on the move direction
    /// </summary>
    private void FlipPlayer()
    {
        if (moveInput.x > 0)
            transform.parent.localScale = new Vector3(1, 1, 1);
        else if (moveInput.x < 0)
            transform.parent.localScale = new Vector3(-1, 1, 1);
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

        Vector2 targetVelocity = new(moveInput.x * _settings.moveSpeed, _rigidbody.linearVelocityY);

        // Calculate the force to apply
        Vector2 force = new((targetVelocity.x - _rigidbody.linearVelocityX) * _settings.acceleration, 0);

        _rigidbody.AddForce(force, ForceMode2D.Force);

        // Clamp the velocity to the max speed
        if (Mathf.Abs(_rigidbody.linearVelocityX) > _settings.maxSpeed)
        {
            _rigidbody.linearVelocity = new(Mathf.Sign(_rigidbody.linearVelocityX) * _settings.maxSpeed, _rigidbody.linearVelocityY);
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from input events
        PlayerInputHandler.OnMove -= OnMoveInput;
    }
}