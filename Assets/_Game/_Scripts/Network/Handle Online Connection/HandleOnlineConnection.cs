using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using UnityEngine.SceneManagement;

public class HandleOnlineConnection : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Host UI")]
    [SerializeField] private TMP_InputField hostJoinCodeInputField;
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button stopHostButton;

    [Header("Client UI")]
    [SerializeField] private TMP_InputField clientJoinCodeInputField;
    [SerializeField] private Button startClientButton;

    private async void Awake()
    {
        await UnityServices.InitializeAsync();

        await SignInAnonymously();

        startHostButton.onClick.AddListener(StartHost);
        stopHostButton.onClick.AddListener(StopHost);
        startClientButton.onClick.AddListener(StartClient);
    }

    private async void StartClient()
    {
        string joinCode = clientJoinCodeInputField.text;

        if (string.IsNullOrEmpty(joinCode))
        {
            statusText.text = "Please enter a join code.";
            return;
        }

        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            // Set up the transport
            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            RelayServerData relayServerData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");
            transport.SetRelayServerData(relayServerData);

            // Start the client
            if (NetworkManager.Singleton.StartClient())
            {
                statusText.text = "Connecting to host...";
            }
            else
            {
                statusText.text = "Failed to start client.";
            }

        }
        catch (RelayServiceException e)
        {
            Debug.LogError($"Failed to join relay: {e.Message}");
            statusText.text = "Failed to join relay.";
            return;
        }
    }

    private void StopHost()
    {
        if (!NetworkManager.Singleton.IsHost)
        {
            statusText.text = "Host is not running.";
            return;
        }

        NetworkManager.Singleton.Shutdown();
        hostJoinCodeInputField.text = string.Empty;
        statusText.text = "Host stopped.";
    }

    private async void StartHost()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            statusText.text = "Host is already running.";
            return;
        }

        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);

            // Get the join code for the allocation
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            hostJoinCodeInputField.text = joinCode;

            // Set up the transport
            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

            RelayServerData relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");
            transport.SetRelayServerData(relayServerData);

            // Start the host
            if (NetworkManager.Singleton.StartHost())
            {
                statusText.text = "Waiting for players...";
                NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoaded;
            }
            else
            {
                statusText.text = "Failed to start host.";
            }
        }
        catch (RelayServiceException e)
        {
            Debug.LogError($"Failed to create relay allocation: {e.Message}");
            statusText.text = "Failed to start host.";
            return;
        }
    }

    private async void OnSceneLoaded(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        if (!NetworkManager.Singleton.IsServer) return;
        if (sceneName == "Menu") return;

        NetworkObject existingPlayer = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId);
        // If the player already exists, do not spawn a new one
        if (existingPlayer != null) return;

        await Task.Delay(500);
        GameObject player = NetworkManager.Singleton.NetworkConfig.Prefabs.Prefabs[0].Prefab;
        NetworkObject playerObject = Instantiate(player).GetComponent<NetworkObject>();
        playerObject.SpawnAsPlayerObject(clientId, true);
    }

    private async Task SignInAnonymously()
    {
        if (AuthenticationService.Instance.IsSignedIn) return;

        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Signed in anonymously. Player ID: " + AuthenticationService.Instance.PlayerId);
        }
        catch (AuthenticationException e)
        {
            Debug.LogError($"Failed to sign in: {e.Message}");
        }
    }

    private void OnDestroy()
    {
        startHostButton.onClick.RemoveListener(StartHost);
        stopHostButton.onClick.RemoveListener(StopHost);
        startClientButton.onClick.RemoveListener(StartClient);
    }
}
