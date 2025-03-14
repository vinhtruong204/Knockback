using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public NetworkVariable<int> TeamId = new NetworkVariable<int>(0);   
    [SerializeField] private int health = 100;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            AssignTeam();
        }
    }

    private void AssignTeam()
    {
        TeamId.Value = (int)OwnerClientId % 2;
    }

    public void TakeDamage(int damage)
    {
        if (!IsServer) return;

        health -= damage;

        if (health <= 0)
        {
            GetComponent<NetworkObject>().Despawn();
        }
    }
}
