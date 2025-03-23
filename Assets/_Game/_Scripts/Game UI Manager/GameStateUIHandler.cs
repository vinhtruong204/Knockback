using Unity.Netcode;
using UnityEngine;
using System.Collections;
using System;

public class GameStateUIHandler : NetworkBehaviour
{
    private GameObject player;
    [SerializeField] private GameObject gameOverUI;

    protected override void OnInSceneObjectsSpawned()
    {
        base.OnNetworkPostSpawn();
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

        player.GetComponentInChildren<PlayerDamageReceiver>().MatchOver += OnMatchOverHandler;
    }

    private void OnMatchOverHandler(bool isWinner)
    {
        if (isWinner)
        {
            OnGameWin();
        }
        else
        {
            OnGameOver();
        }
    }

    private void OnGameWin()
    {
        gameOverUI.SetActive(true); 
        Debug.Log("Game Win" + "KillCount: " + player.GetComponentInChildren<PlayerDamageReceiver>().KillCount + " DeadCount: " + player.GetComponentInChildren<PlayerDamageReceiver>().DeadCount);
    }

    private void OnGameOver()
    {
        gameOverUI.SetActive(true);
        Debug.Log("Game Over" + "KillCount: " + player.GetComponentInChildren<PlayerDamageReceiver>().KillCount + " DeadCount: " + player.GetComponentInChildren<PlayerDamageReceiver>().DeadCount);
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        // Unsubscribe from events to prevent memory leaks
        if (player != null)
        {
            // Unsubscribe from the event to prevent memory leaks
            player.GetComponentInChildren<PlayerDamageReceiver>().MatchOver -= OnMatchOverHandler;
        }
    }
}
