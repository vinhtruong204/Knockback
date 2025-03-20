using System;
using System.Collections;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class MeleeWeapon : WeaponBase
{
    public int Damage { get; private set; }
    public float AttackSpeed { get; private set; }
    public float AttackDuration { get; private set; }

    private float lastTimeAttack;

    private BoxCollider2D boxCollider2D;
    
    [SerializeField] private PlayerTeamId playerTeamId;

    // Send event to update animation
    public event Action<float> OnAttack;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();

        Damage = 10;
        AttackSpeed = 1.0f;
        lastTimeAttack = -AttackSpeed;

        AttackDuration = 1.0f;

    }

    private void Start()
    {
        GetTeamId();   
    }

    private void GetTeamId()
    {
        if (playerTeamId == null)
        {
            Debug.LogError("Player component not found on parent!");
            return;
        }
    }

    public override void Attack()
    {
        if (!CanAttack()) return;

        OnAttack?.Invoke(AttackDuration);
        lastTimeAttack = Time.time;

        // Enable collider to detect enemy
        StartCoroutine(StartEnableCollider());
    }

    private IEnumerator StartEnableCollider()
    {
        boxCollider2D.enabled = true;
        yield return new WaitForSeconds(AttackDuration);
        boxCollider2D.enabled = false;
    }

    /// <summary>
    /// Check if the melee weapon can attack.
    /// </summary>
    /// <remarks>
    /// The melee weapon can attack if the time since the last attack is greater than
    /// or equal to the attack speed.
    /// </remarks>
    public override bool CanAttack()
    {
        return Time.time - lastTimeAttack >= AttackSpeed;
    }

    /// <summary>
    /// Called when another object starts touching this object's collider.
    /// This is used to detect if the melee attack hits an enemy.
    /// </summary>
    /// <param name="collision">The other object that starts touching this object</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Contains("Player"))
        {
            Debug.Log(Name + " hits " + collision.name + " and deals " + Damage + " damage!");

            // Check if collsion with teamate
            PlayerTeamId hitPlayer = collision.GetComponentInChildren<PlayerTeamId>();
            if (hitPlayer == null || hitPlayer.TeamId == playerTeamId.TeamId) return;

            HandleDamage(collision);

            SendForceRpc(collision.transform, Damage * Vector2.right);
        }

        boxCollider2D.enabled = false;
    }


    private void SendForceRpc(Transform transform, Vector2 force)
    {
        // throw new NotImplementedException();
    }

    private void HandleDamage(Collider2D collision)
    {
        PlayerDamageReceiver playerHealth = collision.GetComponentInChildren<PlayerDamageReceiver>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(Damage);
        }
        else
        {
            Debug.Log("No PlayerDamageReceiver found on " + collision.name);
        }
    }
}
