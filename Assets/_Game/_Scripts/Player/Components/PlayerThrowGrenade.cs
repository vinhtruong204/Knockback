using Unity.Netcode;
using UnityEngine;

public class PlayerThrowGrenade : NetworkBehaviour
{
    private int grenadeCount = 3;
    private PlayerInputHandler playerInputHandler;
    private PlayerTeamId playerTeamId;

    [SerializeField]
    private GameObject grenadePrefab;

    private void Start()
    {
        playerInputHandler = transform.parent.GetComponentInChildren<PlayerInputHandler>();
        playerTeamId = transform.parent.GetComponentInChildren<PlayerTeamId>();
        
        playerInputHandler.OnThrow += ThrowGrenade;
    }

    private void ThrowGrenade()
    {
        // Request to throw a bomb
        if (grenadeCount > 0)
        {
            grenadeCount--;

            RequestThrowGrenadeServerRpc(playerTeamId.TeamId);
        }
    }

    [Rpc(SendTo.Server)]
    private void RequestThrowGrenadeServerRpc(int teamId)
    {
        // Get a grenade from the pool
        NetworkObject grenade = NetworkObjectPool.Singleton.GetNetworkObject(grenadePrefab, transform.parent.position, Quaternion.identity);

        // Initialize grenade with owner and team ID
        grenade.GetComponent<Grenade>().Initialize(teamId, transform.parent.localScale);

        // Spawn the grenade on the network
        grenade.Spawn();
    }

    private void OnDisable()
    {
        playerInputHandler.OnThrow -= ThrowGrenade;
    }
}
