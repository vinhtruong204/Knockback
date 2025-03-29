using System;
using System.Collections;
using Unity.Netcode;
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

        Type = WeaponType.Melee;
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
            // Check if collsion with teamate
            PlayerTeamId hitPlayer = collision.GetComponentInChildren<PlayerTeamId>();
            if (hitPlayer == null || hitPlayer.TeamId == playerTeamId.TeamId) return;

            HandleDamageServerRpc(collision.GetComponent<NetworkObject>(), playerTeamId.OwnerClientId);
            
            // Attack direction
            Vector2 attackDirection = playerTeamId.transform.parent.localScale.x > 0 ? Vector2.right : Vector2.left;
            SendForceServerRpc(collision.GetComponent<NetworkObject>(), Damage * attackDirection);
        }

        boxCollider2D.enabled = false;
    }

    /// <summary>
    /// Send a force to the target object.
    /// This is used to push the enemy back when hit by the melee weapon.
    /// </summary>
    /// <param name="targetObject"></param>
    /// <param name="force"></param>
    [Rpc(SendTo.ClientsAndHost)]
    private void SendForceServerRpc(NetworkObjectReference targetObject, Vector2 force)
    {
        if (targetObject.TryGet(out NetworkObject networkObject))
        {
            Rigidbody2D rb = networkObject.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.AddForce(force, ForceMode2D.Impulse);
            }
            else
            {
                Debug.Log("No Rigidbody2D found on " + networkObject.name);
            }
        }
    }

    [Rpc(SendTo.Server)]
    private void HandleDamageServerRpc(NetworkObjectReference targetObject, ulong attackerId)
    {
        if (targetObject.TryGet(out NetworkObject networkObject))
        {
            PlayerDamageReceiver playerHealth = networkObject.GetComponentInChildren<PlayerDamageReceiver>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(Damage, attackerId);
            }
            else
            {
                Debug.Log("No PlayerDamageReceiver found on " + networkObject.name);
            }
        }
    }
}
