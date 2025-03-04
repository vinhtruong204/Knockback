using System.Threading.Tasks;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    private PlayerJumpConfigSO _settings;
    private Rigidbody2D _rigidbody;
    private int jumpCount;

    private async void Awake()
    {
        await LoadPlayerSettings();

        InitializeRigidbody();
    }

    private async Task LoadPlayerSettings()
    {
        var (asset, handle) = await AddressableLoader<PlayerJumpConfigSO>.LoadAssetAsync("PlayerJumpConfigSO");

        _settings = asset;
        AddressableLoader<PlayerJumpConfigSO>.ReleaseHandle(handle);
    }

    private void OnEnable()
    {
        // Subscribe to input events
        PlayerInputHandler.OnJump += OnJumpInput;
    }

    private void InitializeRigidbody()
    {
        _rigidbody = GetComponentInParent<Rigidbody2D>();
        _rigidbody.gravityScale = _settings.gravityScale;
    }

    /// <summary>
    /// Get input from class player input
    /// </summary>
    private void OnJumpInput()
    {
        JumpPlayer();
    }

    /// <summary>
    /// Jump the player based on input and settings
    /// </summary>
    private void JumpPlayer()
    {
        if (jumpCount >= _settings.maxJumps) return;

        _rigidbody.AddForce(Vector2.up * _settings.jumpForce, ForceMode2D.Impulse);
        jumpCount++;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            jumpCount = 0;
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from input events
        PlayerInputHandler.OnJump -= OnJumpInput;
    }
}
