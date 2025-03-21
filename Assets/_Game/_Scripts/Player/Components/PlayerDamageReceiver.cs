using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerDamageReceiver : NetworkBehaviour
{

    private const int MAX_HEALTH = 100;
    private const int MAX_HEART = 5;
    public NetworkVariable<int> health = new NetworkVariable<int>(
        MAX_HEALTH,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    public NetworkVariable<int> heart = new NetworkVariable<int>(
        MAX_HEART,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public int Health => health.Value;
    public int Heart => heart.Value;

    public event Action<int, int> HealthChanged;
    public event Action<int> HeartChanged;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        health.OnValueChanged += OnHealthChangedHandler;
        heart.OnValueChanged += OnHeartChangedHandler;
    }

    private void OnHeartChangedHandler(int previousHeart, int newHeart)
    {
        HeartChanged?.Invoke(newHeart);
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

        health.Value -= 50;

        if (health.Value <= 0)
        {
            ResetPositionOnDeathClientRpc();
            
            heart.Value -= 1;
            health.Value = MAX_HEALTH;
        }
    }

    /// <summary>
    /// Resets the player's position and velocity when they die.
    /// </summary>
    /// <remarks>
    /// This method is called on the clients when the player's health reaches 0.
    /// </remarks>
    [Rpc(SendTo.ClientsAndHost)]
    private void ResetPositionOnDeathClientRpc()
    {
        transform.parent.position = Vector3.zero;
        transform.parent.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        health.OnValueChanged -= OnHealthChangedHandler;
        heart.OnValueChanged -= OnHeartChangedHandler;
    }
}
