using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameStateUIHandler : NetworkBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject matchCompletedUI;
    [SerializeField] private TextMeshProUGUI killCountText;
    [SerializeField] private TextMeshProUGUI deadCountText;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Button okButton;

    protected override void OnInSceneObjectsSpawned()
    {
        base.OnInSceneObjectsSpawned();

        LoadPlayerObject();
    }

    private async void LoadPlayerObject()
    {
        await Task.Delay(1000);

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
            resultText.text = "You Win!";
        }
        else
        {
            resultText.text = "You Lose!";
        }

        matchCompletedUI.SetActive(true);
        killCountText.text = "Kill Count: " + player.GetComponentInChildren<PlayerDamageReceiver>().KillCount.ToString();
        deadCountText.text = "Dead Count: " + player.GetComponentInChildren<PlayerDamageReceiver>().DeadCount.ToString();
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
