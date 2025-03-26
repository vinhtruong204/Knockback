using System;
using System.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class PlayerDamageReceiver : NetworkBehaviour
{
    private const int MAX_HEALTH = 100;
    private const int MAX_HEART = 5;
    private Vector3 revivedPosition = new Vector3(0, 7, 0); // Set the position where the player will be revived

    private NetworkVariable<int> health = new NetworkVariable<int>(
        MAX_HEALTH,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    private NetworkVariable<int> heart = new NetworkVariable<int>(
        MAX_HEART,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    public NetworkVariable<int> killCount = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    private NetworkVariable<int> deadCount = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    // Lưu ID của người chơi gây sát thương cuối cùng
    public NetworkVariable<ulong> lastAttackerId = new NetworkVariable<ulong>(
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
                StartCoroutine(NotifyMatchOverAfterDelay(this, false));
            }
            else
            {
                OnMatchCompletedOnWinner();
            }

            GetComponentInParent<Rigidbody2D>().simulated = false;
        }
    }

    /// <summary>
    /// Handles the match completion for the winner.
    /// </summary>
    private void OnMatchCompletedOnWinner()
    {
        GameObject player = null;
        ulong localClientId = NetworkManager.Singleton.LocalClientId;

        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(localClientId, out NetworkClient client))
        {
            player = client.PlayerObject != null ? client.PlayerObject.gameObject : null;
        }

        if (player == null)
        {
            Debug.LogError("Player object not found.");
            return;
        }

        // Send win message to the local player
        PlayerDamageReceiver damageReceiver = player.GetComponentInChildren<PlayerDamageReceiver>();
        if (damageReceiver != null)
        {
            StartCoroutine(NotifyMatchOverAfterDelay(damageReceiver, true));
        }
        else
        {
            Debug.LogError("PlayerDamageReceiver not found.");
        }
    }

    private IEnumerator NotifyMatchOverAfterDelay(PlayerDamageReceiver damageReceiver, bool isWinner)
    {
        yield return new WaitForSeconds(1f); // Delay to ensure the player is fully initialized
        damageReceiver.MatchOver?.Invoke(isWinner);
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

        health.Value -= damage;
        lastAttackerId.Value = attackerId;

        if (health.Value <= 0)
        {
            LostLife();
        }
    }

    public void LostLife()
    {
        heart.Value -= 1;
        health.Value = MAX_HEALTH;
        ResetPositionOnDeathClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ResetPositionOnDeathClientRpc()
    {
        transform.parent.position = revivedPosition;
        transform.parent.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        health.OnValueChanged -= OnHealthChangedHandler;
        heart.OnValueChanged -= OnHeartChangedHandler;
    }
}
