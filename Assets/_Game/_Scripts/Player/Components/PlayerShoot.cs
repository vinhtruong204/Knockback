using Unity.Netcode;
using UnityEngine;

public class PlayerShoot : NetworkBehaviour
{
    private PlayerInputHandler playerInputHandler;
    [SerializeField] private GameObject bulletPrefab;
    private PlayerTeamId playerTeamId; // Reference to the Player script

    private async void Awake()
    {
        // Load bullet prefab if prefab not assigned in editor
        if (bulletPrefab == null)
        {
            Debug.LogWarning("Prefab not assigned. Loading bullet prefab...");
            var (prefab, handle) = await AddressableLoader<GameObject>.LoadAssetAsync("Bullet");

            bulletPrefab = prefab;
            AddressableLoader<GameObject>.ReleaseHandle(handle);
        }
    }

    private void Start()
    {
        playerInputHandler = transform.parent.GetComponentInChildren<PlayerInputHandler>();
        playerTeamId = transform.parent.GetComponentInChildren<PlayerTeamId>();

        if (playerTeamId == null)
        {
            Debug.LogError("Player component not found on parent!");
        }

        // Subscribe to input events
        playerInputHandler.OnShoot += OnShoot;
    }

    private void OnShoot()
    {
        RequestShootRpc(playerTeamId.TeamId);
    }

    [Rpc(SendTo.Server)]
    private void RequestShootRpc(int teamId)
    {
        NetworkObject bullet = NetworkObjectPool.Singleton.GetNetworkObject(bulletPrefab, transform.GetChild(0).position, Quaternion.identity);
        bullet.transform.localScale = transform.parent.localScale;

        // Initialize bullet with owner and team ID
        bullet.GetComponent<Bullet>().Initialize(teamId, transform.parent.localScale);

        // Spawn the bullet on the network
        bullet.GetComponent<NetworkObject>().Spawn();
    }

    private void OnDisable()
    {
        if (playerInputHandler != null)
            playerInputHandler.OnShoot -= OnShoot;
    }
}