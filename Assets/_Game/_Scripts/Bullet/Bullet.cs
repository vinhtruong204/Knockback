using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour, IDisableAfterTime
{
    [SerializeField] private float speed = 10f;
    private int damage = 10;
    private Vector2 moveDirection;
    private float timeToReturnPool = 2f;
    private int bulletTeamId;

    /// <summary>
    /// Initialize the bullet
    /// </summary>
    /// <remarks>
    /// Only the server can call this method
    /// </remarks>
    /// <param name="teamId">The team this bullet belongs to</param>
    /// <param name="scale">The scale of the bullet</param>
    public void Initialize(int teamId, Vector3 scale)
    {
        bulletTeamId = teamId;
        transform.localScale = scale;
        moveDirection = transform.localScale.x < 0f ? Vector2.left : Vector2.right;
        StartCoroutine(DisableAfterTime());
    }

    public IEnumerator DisableAfterTime()
    {
        yield return new WaitForSeconds(timeToReturnPool);

        if (IsServer) GetComponent<NetworkObject>().Despawn();
    }

    private void Update()
    {
        if (IsServer)
            transform.Translate(speed * Time.deltaTime * moveDirection);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) return; // Only server handles collisions

        // Check if collsion with teamate
        PlayerTeamId hitPlayer = collision.GetComponentInChildren<PlayerTeamId>();
        if (hitPlayer == null || hitPlayer.TeamId == bulletTeamId) return;

        // Check if collision with player
        PlayerDamageReceiver hitPlayerHealth = collision.GetComponentInChildren<PlayerDamageReceiver>();
        if (hitPlayerHealth == null)
        {
            Debug.Log("No PlayerDamageReceiver found on " + collision.name);
            return;
        }

        hitPlayerHealth.TakeDamage(damage, OwnerClientId);

        // Check if collision with other player
        if (collision.TryGetComponent(out NetworkObject networkObject) && networkObject.IsSpawned)
        {
            ApplyForceRpc(networkObject, moveDirection * speed);
        }

        GetComponent<NetworkObject>().Despawn();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ApplyForceRpc(NetworkObjectReference objectRef, Vector2 force)
    {
        if (objectRef.TryGet(out NetworkObject networkObject))
        {
            if (networkObject.TryGetComponent(out Rigidbody2D rb))
            {
                rb.AddForce(force, ForceMode2D.Impulse);
            }
        }
    }
}