using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerDamageReceiver : NetworkBehaviour
{
    private const int MAX_HEALTH = 100;
    private const int MAX_HEART = 5;

    private NetworkVariable<int> health = new NetworkVariable<int>(
        MAX_HEALTH,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    private NetworkVariable<int> heart = new NetworkVariable<int>(
        MAX_HEART,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    private NetworkVariable<int> killCount = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    private NetworkVariable<int> deadCount = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    // Lưu ID của người chơi gây sát thương cuối cùng
    private NetworkVariable<ulong> lastAttackerId = new NetworkVariable<ulong>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    public int KillCount => killCount.Value;
    public int DeadCount => deadCount.Value;
    public int Health => health.Value;
    public int Heart => heart.Value;

    public event Action<int, int> HealthChanged;
    public event Action<int> HeartChanged;

    public delegate void GameOverHandler(bool isWinner);
    public event GameOverHandler MatchOver;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        health.OnValueChanged += OnHealthChangedHandler;
        heart.OnValueChanged += OnHeartChangedHandler;
    }

    private void OnHeartChangedHandler(int previousHeart, int newHeart)
    {
        HeartChanged?.Invoke(newHeart);

        if (IsOwner)
        {
            Debug.Log("Owner heart changed: " + newHeart);
            deadCount.Value += 1;

            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(lastAttackerId.Value, out NetworkClient attackerClient))
            {
                SendKillCountToAttackerServerRpc(attackerClient.PlayerObject);
            }
            else
            {
                Debug.LogError("AttackerClient is null");
            }
        }

        if (newHeart <= 0)
        {
            if (IsOwner)
            {
                MatchOver?.Invoke(false);
            }
            else
            {
                MatchOver?.Invoke(true);
            }
        }
    }

    [Rpc(SendTo.Server)]
    private void SendKillCountToAttackerServerRpc(NetworkObjectReference playerObjectReference)
    {
        if (!playerObjectReference.TryGet(out NetworkObject playerNetworkObject))
        {
            Debug.LogError("PlayerObjectReference is null");
            return;
        }

        PlayerDamageReceiver attackerDamageReceiver = playerNetworkObject.GetComponentInChildren<PlayerDamageReceiver>();

        if (attackerDamageReceiver == null)
        {
            Debug.LogError("AttackerDamageReceiver is null");
            return;
        }

        attackerDamageReceiver.AddKillCount();
    }

    private void AddKillCount()
    {
        killCount.Value += 1;
    }

    private void OnHealthChangedHandler(int previousHealth, int newHealth)
    {
        HealthChanged?.Invoke(newHealth, MAX_HEALTH);
    }

    /// <summary>
    /// Handles the damage taken by the player.
    /// </summary>
    public void TakeDamage(int damage, ulong attackerId)
    {
        if (!IsServer) return;

        health.Value -= 50;

        if (health.Value <= 0)
        {
            lastAttackerId.Value = attackerId;

            ResetPositionOnDeathClientRpc();
            heart.Value -= 1;
            health.Value = MAX_HEALTH;
        }
    }

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
