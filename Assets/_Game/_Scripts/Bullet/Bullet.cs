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
        
        NetworkObjectPool.Singleton.ReturnNetworkObject(GetComponent<NetworkObject>(), bulletPrefab);
    }

    private void Update()
    {
        transform.Translate(_moveDirection * _speed * Time.deltaTime);
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
            NetworkObjectPool.Singleton.ReturnNetworkObject(GetComponent<NetworkObject>(), bulletPrefab);
            GetComponent<NetworkObject>().Despawn();
        }

        // If it hits something with a Rigidbody2D (but not a player), apply force
        if (collision.TryGetComponent(out Rigidbody2D rb))
        {
            rb.AddForce(_moveDirection * _speed, ForceMode2D.Impulse);
            NetworkObjectPool.Singleton.ReturnNetworkObject(GetComponent<NetworkObject>(), bulletPrefab);
            GetComponent<NetworkObject>().Despawn();
        }
    }
}