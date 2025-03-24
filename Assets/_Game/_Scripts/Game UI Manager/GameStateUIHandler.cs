using Unity.Netcode;
using UnityEngine;

public class GameStateUIHandler : NetworkBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject matchCompletedUI;

    protected override void OnInSceneObjectsSpawned()
    {
        base.OnInSceneObjectsSpawned();
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

        PlayerDamageReceiver damageReceiver = player.GetComponentInChildren<PlayerDamageReceiver>();
        if (damageReceiver != null)
        {
            damageReceiver.MatchOver += OnMatchOverHandler;
        }
        else
        {
            Debug.LogError("PlayerDamageReceiver not found in player object.");
        }
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
        matchCompletedUI.SetActive(true);
        Debug.Log("Game Win" + "KillCount: " + player.GetComponentInChildren<PlayerDamageReceiver>().KillCount + " DeadCount: " + player.GetComponentInChildren<PlayerDamageReceiver>().DeadCount);
    }

    private void OnGameOver()
    {
        matchCompletedUI.SetActive(true);
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
