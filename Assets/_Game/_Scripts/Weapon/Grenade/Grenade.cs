using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Grenade : NetworkBehaviour, IDisableAfterTime
{
    private float timeToReturnPoolMax = 2f;
    private float explosionDuration = 0.2f;
    private float throwForce = 5f;
    private float explosionForce = 10f;
    private int damage = 10;
    private Rigidbody2D grenadeRigidbody;
    private CircleCollider2D grenadeCollider;
    private int teamId;

    private void Awake()
    {
        grenadeRigidbody = GetComponent<Rigidbody2D>();
        grenadeCollider = GetComponent<CircleCollider2D>();
    }

    public override void OnNetworkSpawn()
    {
        if (transform.localScale.x < 0f)
            grenadeRigidbody.AddForce(Vector2.left * throwForce, ForceMode2D.Impulse);
        else if (transform.localScale.x > 0f)
            grenadeRigidbody.AddForce(Vector2.right * throwForce, ForceMode2D.Impulse);

        grenadeCollider.enabled = false;
        StartCoroutine(ExplodeAfterTime());
    }

    private IEnumerator ExplodeAfterTime()
    {
        yield return new WaitForSeconds(timeToReturnPoolMax);

        Explode();
    }

    private void Explode()
    {
        grenadeCollider.enabled = true;
        StartCoroutine(DisableAfterTime());
    }

    public void Initialize(int teamId, Vector3 scale)
    {
        this.teamId = teamId;
        transform.localScale = scale;
    }

    public IEnumerator DisableAfterTime()
    {
        yield return new WaitForSeconds(explosionDuration);
        grenadeCollider.enabled = false;

        if (IsServer && GetComponent<NetworkObject>().IsSpawned)
            GetComponent<NetworkObject>().Despawn();
    }

    // Event raised on server when another object starts touching this object's collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) return;
        if (!collision.name.Contains("Player")) return;

        Debug.Log("Grenade hits " + collision.name);

        // Check if collsion with teamate
        PlayerTeamId hitPlayer = collision.GetComponentInChildren<PlayerTeamId>();
        if (hitPlayer == null) return;

        // Check if collision with player
        PlayerDamageReceiver hitPlayerHealth = collision.GetComponentInChildren<PlayerDamageReceiver>();
        if (hitPlayerHealth == null)
        {
            Debug.Log("No PlayerDamageReceiver found on " + collision.name);
            return;
        }

        if (hitPlayer.TeamId == teamId)
        {
            hitPlayerHealth.TakeDamage(damage / 2);
        }
        else
        {
            hitPlayerHealth.TakeDamage(damage);
        }

        // Check if collision with other player
        if (collision.GetComponent<NetworkObject>().IsSpawned)
            ApplyForceClientRpc(collision.GetComponent<NetworkObject>(), Vector2.up * explosionForce);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ApplyForceClientRpc(NetworkObjectReference networkReference, Vector2 force)
    {
        if (networkReference.TryGet(out NetworkObject networkObject))
        {
            if (networkObject.TryGetComponent(out Rigidbody2D playerRigidbody))
            {
                playerRigidbody.AddForce(force, ForceMode2D.Impulse);
            }
        }
    }
}
