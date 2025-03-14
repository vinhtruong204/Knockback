using Unity.Netcode;
using UnityEngine;

public class PlayerShoot : NetworkBehaviour
{
    private PlayerInputHandler playerInputHandler;
    [SerializeField] private GameObject bulletPrefab;
    private Player player; // Reference to the Player script

    private void Start()
    {
        playerInputHandler = transform.parent.GetComponentInChildren<PlayerInputHandler>();
        player = transform.parent.GetComponent<Player>(); // Get the Player script
        if (player == null)
        {
            Debug.LogError("Player component not found on parent!");
        }

        // Subscribe to input events
        playerInputHandler.OnShoot += OnShoot;
    }

    private void OnShoot()
    {
        RequestShootRpc(player.TeamId.Value);
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