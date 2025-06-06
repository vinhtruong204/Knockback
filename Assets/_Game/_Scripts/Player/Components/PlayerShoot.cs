using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerShoot : NetworkBehaviour
{
    private PlayerTeamId playerTeamId; // Reference to the Player script

    private PlayerInputHandler playerInputHandler;
    private WeaponManager weaponManager;
    [SerializeField] private GameObject bulletPrefab;

    public event Action OnShoot;

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

        weaponManager = transform.parent.GetComponentInChildren<WeaponManager>();
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
        playerInputHandler.OnAttack += OnAttack;
    }

    private void OnAttack()
    {
        if (!weaponManager.CurrentWeapon.CanAttack()) return;

        weaponManager.CurrentWeapon.Attack();

        if (weaponManager.CurrentWeapon is IFireable)
        {
            RequestShootRpc(playerTeamId.TeamId, OwnerClientId);
            OnShoot?.Invoke(); // Notify subscribers that the player has shot
        }
    }

    [Rpc(SendTo.Server)]
    private void RequestShootRpc(int teamId, ulong shoterId)
    {
        // Get a bullet from the pool
        NetworkObject bullet = NetworkObjectPool.Singleton.GetNetworkObject(bulletPrefab, transform.GetChild(0).position, Quaternion.identity);

        // Initialize bullet with owner and team ID
        bullet.GetComponent<Bullet>().Initialize(teamId, transform.parent.localScale);

        // Spawn the bullet on the network
        bullet.GetComponent<NetworkObject>().SpawnWithOwnership(shoterId);
    }

    private void OnDisable()
    {
        if (playerInputHandler != null)
            playerInputHandler.OnAttack -= OnAttack;
    }
}