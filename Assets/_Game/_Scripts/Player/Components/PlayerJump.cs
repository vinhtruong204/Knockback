using System.Threading.Tasks;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    private CheckOnGround checkOnGround;
    private PlayerInputHandler playerInputHandler;
    private PlayerJumpConfigSO settings;
    private Rigidbody2D playerRigidbody;
    private int jumpCount;

    private async void Awake()
    {
        await LoadPlayerSettings();

        InitializeRigidbody();
    }


    private void OnGrounded()
    {
        // Reset jump count when the player is grounded
        
        jumpCount = 0;
    }

    private async Task LoadPlayerSettings()
    {
        var (asset, handle) = await AddressableLoader<PlayerJumpConfigSO>.LoadAssetAsync("PlayerJumpConfigSO");

        settings = asset;
        AddressableLoader<PlayerJumpConfigSO>.ReleaseHandle(handle);
    }

    private void Start()
    {
        playerInputHandler = transform.parent.GetComponentInChildren<PlayerInputHandler>();

        // Subscribe to input events
        playerInputHandler.OnJump += OnJumpInput;

        // Subscribe to ground check events
        checkOnGround = transform.parent.GetComponentInChildren<CheckOnGround>();
        checkOnGround.OnGrounded += OnGrounded;
    }

    private void InitializeRigidbody()
    {
        playerRigidbody = GetComponentInParent<Rigidbody2D>();
        playerRigidbody.gravityScale = settings.gravityScale;
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
        if (jumpCount >= settings.maxJumps) return;

        playerRigidbody.AddForce(Vector2.up * settings.jumpForce, ForceMode2D.Impulse);
        jumpCount++;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from input events
        playerInputHandler.OnJump -= OnJumpInput;

        checkOnGround.OnGrounded -= OnGrounded;
    }
}
