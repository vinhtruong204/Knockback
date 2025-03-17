using System;
using Unity.Netcode;

public class PlayerDamageReceiver : NetworkBehaviour
{
    private const int MAX_HEALTH = 100;
    private NetworkVariable<int> health = new NetworkVariable<int>(
        MAX_HEALTH,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    public int Health => health.Value;

    public event Action<int, int> HealthChanged;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        health.OnValueChanged += OnHealthChangedHandler;
    }

    private void OnHealthChangedHandler(int previousHealth, int newHealth)
    {
        HealthChanged?.Invoke(newHealth, MAX_HEALTH);
    }

    /// <summary>
    /// Occurs when the player's health changes.
    /// </summary>
    /// <param name="newHealth">The new health value of the player.</param>
    /// Only the server can call this method
    /// </remarks>
    /// <param name="damage">The amount of damage to take</param>
    public void TakeDamage(int damage)
    {
        if (!IsServer) return;

        health.Value -= damage;

        if (health.Value <= 0)
        {
            GetComponentInParent<NetworkObject>().Despawn();
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        health.OnValueChanged -= OnHealthChangedHandler;
    }
}
