using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerThrowGrenade : NetworkBehaviour
{
    private const int MAX_GRENADE_COUNT = 3;
    private int grenadeCount = MAX_GRENADE_COUNT;
    private PlayerInputHandler playerInputHandler;
    private PlayerTeamId playerTeamId;

    [SerializeField]
    private GameObject grenadePrefab;

    public event Action<int> OnThrowGrenade;

    private void Start()
    {
        playerInputHandler = transform.parent.GetComponentInChildren<PlayerInputHandler>();
        playerTeamId = transform.parent.GetComponentInChildren<PlayerTeamId>();
        
        playerInputHandler.OnThrow += ThrowGrenade;
    }

    public void ResetGrenadeCount()
    {
        grenadeCount = MAX_GRENADE_COUNT;

        // Raise event to update grenade ui
        OnThrowGrenade?.Invoke(grenadeCount);
    }

    private void ThrowGrenade()
    {
        // Request to throw a bomb
        if (grenadeCount > 0)
        {
            grenadeCount--;

            OnThrowGrenade?.Invoke(grenadeCount);

            RequestThrowGrenadeServerRpc(playerTeamId.TeamId, OwnerClientId);
        }
    }

    [Rpc(SendTo.Server)]
    private void RequestThrowGrenadeServerRpc(int teamId, ulong grenadeOwnerId)
    {
        // Get a grenade from the pool
        NetworkObject grenade = NetworkObjectPool.Singleton.GetNetworkObject(grenadePrefab, transform.parent.position, Quaternion.identity);

        // Initialize grenade with owner and team ID
        grenade.GetComponent<Grenade>().Initialize(teamId, transform.parent.localScale);

        // Spawn the grenade on the network
        grenade.SpawnWithOwnership(grenadeOwnerId);
    }

    private void OnDisable()
    {
        playerInputHandler.OnThrow -= ThrowGrenade;
    }
}
