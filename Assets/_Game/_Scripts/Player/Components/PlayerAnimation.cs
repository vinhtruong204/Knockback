using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private CheckOnGround checkOnGround;
    private PlayerInputHandler playerInputHandler;

    // 
    [SerializeField] private Transform rightHand; // Right hand hold the weapons

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

    private void Start()
    {
        playerInputHandler = transform.parent.GetComponentInChildren<PlayerInputHandler>();

        playerInputHandler.OnMove += HandleMove;
        playerInputHandler.OnJump += HandleJump;

        // Subscribe to the event on ground
        checkOnGround = transform.parent.GetComponentInChildren<CheckOnGround>();
        checkOnGround.OnGrounded += HandleGrounded;

        // Subscribe to the event on reload
        for (int i = 0; i < rightHand.childCount; i++)
        {
            WeaponBase child = rightHand.GetChild(i).GetComponent<WeaponBase>();

            // Primary and secondary weapon
            if (child is IReloadable)
            {
                (child as IReloadable).OnReload += HandleReload;
            }

            // Melee weapon
            if (child is MeleeWeapon)
            {
                (child as MeleeWeapon).OnAttack += HandleAttack;
            }
        }
    }

    private void HandleAttack(float attackDuration)
    {
        UpdateAnimationType(AnimationType.MeleeAttack);
        StartCoroutine(MeleeAttackCoroutine(attackDuration));
    }

    private IEnumerator MeleeAttackCoroutine(float attackDuration)
    {
        yield return new WaitForSeconds(attackDuration);
        UpdateAnimationType(AnimationType.Idle);
    }

    private void HandleReload(float reloadTime)
    {
        UpdateAnimationType(AnimationType.Reload);
        StartCoroutine(ReloadCoroutine(reloadTime));
    }

    private IEnumerator ReloadCoroutine(float reloadTime)
    {
        yield return new WaitForSeconds(reloadTime);
        UpdateAnimationType(AnimationType.Idle);
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
        playerInputHandler.OnMove -= HandleMove;
        playerInputHandler.OnJump -= HandleJump;

        // Unsubscribe from the event on ground
        checkOnGround.OnGrounded -= HandleGrounded;

        // Unsubscribe from the event on reload
        for (int i = 0; i < rightHand.childCount; i++)
        {
            WeaponBase child = rightHand.GetChild(i).GetComponent<WeaponBase>();

            // Primary and secondary weapon
            if (child is IReloadable)
            {
                (child as IReloadable).OnReload += HandleReload;
            }

            // Melee weapon
            if (child is MeleeWeapon)
            {
                (child as MeleeWeapon).OnAttack += HandleAttack;
            }
        }
    }
}
