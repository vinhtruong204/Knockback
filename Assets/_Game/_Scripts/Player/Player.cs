using System;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public NetworkVariable<int> TeamId = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> health = new NetworkVariable<int>(
        100,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    public event Action<int> HealthChanged;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
            TeamId.Value = (int)OwnerClientId % 2;

        health.OnValueChanged += OnHealthChangedHandler;
    }

    private void OnHealthChangedHandler(int previousValue, int newValue)
    {
        HealthChanged?.Invoke(newValue);
    }

    public void TakeDamage(int damage)
    {
        if (!IsServer) return;

        health.Value -= damage;

        if (health.Value <= 0)
        {
            GetComponent<NetworkObject>().Despawn();
        }
    }
}
