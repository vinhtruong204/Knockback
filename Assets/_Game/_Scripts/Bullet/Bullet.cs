using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour, IDisableAfterTime
{
    [SerializeField] private float _speed = 10f;
    private Vector2 _moveDirection;
    private float _timeToReturnPoolMax = 2f;
    private int bulletTeamId; // Track the team this bullet belongs to
    [SerializeField] GameObject bulletPrefab;

    private async void Start()
    {
        var (bulletPrefab, handlers) = await AddressableLoader<GameObject>.LoadAssetAsync("Bullet");

        this.bulletPrefab = bulletPrefab;
        AddressableLoader<GameObject>.ReleaseHandle(handlers);
    }

    public void Initialize(int teamId, Vector3 scale)
    {
        bulletTeamId = teamId;
        transform.localScale = scale;
        _moveDirection = transform.localScale.x < 0f ? Vector2.left : Vector2.right;
        StartCoroutine(DisableAfterTime());
    }

    public IEnumerator DisableAfterTime()
    {
        yield return new WaitForSeconds(_timeToReturnPoolMax);

        if (IsServer) GetComponent<NetworkObject>().Despawn();
    }

    private void Update()
    {
        transform.Translate(_speed * Time.deltaTime * _moveDirection);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) return; // Only server handles collisions

        // Check if we hit a player
        if (collision.TryGetComponent(out Player hitPlayer))
        {

            // Ignore if it's a teammate
            if (hitPlayer.TeamId.Value == bulletTeamId)
                return;

            // Apply damage to enemy
            hitPlayer.TakeDamage(10); // Example damage value
        }

        // If it hits something with a Rigidbody2D, apply force
        ApplyForceRpc(collision.GetComponent<NetworkObject>(), _moveDirection * _speed);

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