using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private AnimationType currentAnimation;
    private Rigidbody2D rb;
    private bool isJumping;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponentInParent<Rigidbody2D>();

        currentAnimation = AnimationType.Idle;
    }

    private void OnEnable()
    {
        PlayerInputHandler.OnMove += HandleMove;
        PlayerInputHandler.OnJump += HandleJump;

        CheckOnGround.OnGrounded += HandleGrounded;
    }

    private void HandleGrounded()
    {
        isJumping = false;

        // If the player is not moving, set to idle
        AnimationType newAnimation = Mathf.Abs(rb.linearVelocityX) <= 0.1f ? AnimationType.Idle : currentAnimation;
        UpdateAnimationType(newAnimation);
    }

    private void HandleJump()
    {
        if (isJumping) return;

        isJumping = true;   
        UpdateAnimationType(AnimationType.Jump);
    }

    private void HandleMove(Vector2 vector)
    {
        // If the player is jumping, don't change the animation
        if (isJumping) return;

        // If the player is not moving, set to idle
        AnimationType newAnimation = vector == Vector2.zero ? AnimationType.Idle : AnimationType.Run;

        UpdateAnimationType(newAnimation);
    }

    private void UpdateAnimationType(AnimationType newAnimation)
    {
        // Check if the new animation is the same as the current one
        if (currentAnimation == newAnimation) return;

        animator.CrossFade(newAnimation.ToString(), 0.2f, 0);
        currentAnimation = newAnimation;
    }

    private void OnDisable()
    {
        PlayerInputHandler.OnMove -= HandleMove;
        PlayerInputHandler.OnJump -= HandleJump;

        // Unsubscribe from the event on ground
        CheckOnGround.OnGrounded -= HandleGrounded;
    }
}
