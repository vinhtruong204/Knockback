using System;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private AnimationType currentAnimation;
    [SerializeField] private bool isJumping;

    private void Awake()
    {
        animator = GetComponent<Animator>();

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

        // Reset the animation to idle when grounded
        HandleMove(Vector2.zero);
    }

    private void HandleJump()
    {
        if (currentAnimation == AnimationType.Jump) return;

        animator.CrossFade(AnimationType.Jump.ToString(), 0.35f, 0);

        isJumping = true;
        currentAnimation = AnimationType.Jump;
    }

    private void HandleMove(Vector2 vector)
    {
        // If the player is jumping, don't change the animation
        if (isJumping) return;

        // If the player is not moving, set to idle
        AnimationType newAnimation = vector == Vector2.zero ? AnimationType.Idle : AnimationType.Run;

        // Check if the new animation is the same as the current one
        if (currentAnimation == newAnimation) return;

        animator.CrossFade(newAnimation.ToString(), 0.35f, 0);
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
