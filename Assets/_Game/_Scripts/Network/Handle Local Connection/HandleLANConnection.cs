using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HandleLANConnection : MonoBehaviour
{
    [Header("Host UI Elements")]
    [SerializeField] private TMP_InputField ipHostInputField;
    [SerializeField] private TMP_InputField portHostInputField;
    [SerializeField] private Button startHostButton;

    [Header("Client UI Elements")]
    [SerializeField] private TMP_InputField ipClientInputField;
    [SerializeField] private TMP_InputField portClientInputField;
    [SerializeField] private Button startClientButton;
    private const int MAX_PLAYER = 2;

    private void Awake()
    {
        SetupHostUI();

        SetupClientUI();
    }

    private void Start()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private async void OnSceneLoaded(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        var existingPlayer = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId);
        if (existingPlayer != null)
        {
            Debug.Log($"Player {clientId} already has a player object.");
            return;
        }

        await Task.Delay(500);

        GameObject player = NetworkManager.Singleton.NetworkConfig.Prefabs.Prefabs[0].Prefab;
        NetworkObject playerObject = Instantiate(player).GetComponent<NetworkObject>();
        playerObject.SpawnAsPlayerObject(clientId, true);
    }


    private void OnClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton.IsHost
            && NetworkManager.Singleton.ConnectedClients.Count == MAX_PLAYER)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("Main", LoadSceneMode.Single);
        }
    }

    private void SetupClientUI()
    {
        startClientButton.onClick.AddListener(OnStartClientButtonClicked);
    }

    private void SetupHostUI()
    {
        ipHostInputField.text = GetLocalIPAddress();
        portHostInputField.text = "7777";
        startHostButton.onClick.AddListener(OnStartHostButtonClicked);
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        if (NetworkManager.Singleton.ConnectedClients.Count >= MAX_PLAYER)
        {
            response.Approved = false;
            response.Reason = "Max player limit reached.";
        }
        else
        {
            response.Approved = true;
            response.Pending = false;
            response.CreatePlayerObject = false;
        }
    }


    private void OnStartHostButtonClicked()
    {
        string ip = ipHostInputField.text;
        ushort.TryParse(portHostInputField.text, out ushort port);

        if (string.IsNullOrEmpty(ip) || string.IsNullOrEmpty(port.ToString()))
        {
            Debug.LogError("IP or Port cannot be empty.");
            return;
        }

        NetworkManager networkManager = NetworkManager.Singleton;
        if (networkManager != null)
        {
            networkManager.GetComponent<UnityTransport>().SetConnectionData(ip, port);

            // If the host is not already started, start it
            // This is to prevent starting a host multiple times
            if (!NetworkManager.Singleton.IsHost)
            {
                networkManager.StartHost();
                NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoaded;
            }
        }
        else
        {
            Debug.LogError("NetworkManager not found in the scene.");
        }

    }


    private void OnStartClientButtonClicked()
    {
        string ip = ipClientInputField.text;
        ushort.TryParse(portClientInputField.text, out ushort port);

        if (string.IsNullOrEmpty(ip) || string.IsNullOrEmpty(port.ToString()))
        {
            Debug.LogError("IP or Port cannot be empty.");
            return;
        }

        NetworkManager networkManager = NetworkManager.Singleton;
        if (networkManager != null)
        {
            networkManager.GetComponent<UnityTransport>().SetConnectionData(ip, port);

            // If the client is not already started, start it
            if (!NetworkManager.Singleton.IsClient)
            {
                networkManager.StartClient();
            }
        }
        else
        {
            Debug.LogError("NetworkManager not found in the scene.");
        }
    }

    public string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }

        throw new Exception("No network adapters with an IPv4 address in the system!");
    }

    private void OnDestroy()
    {
        startClientButton.onClick.RemoveAllListeners();
        startHostButton.onClick.RemoveAllListeners();
    }
}
