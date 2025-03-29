using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HandleLANConnection : MonoBehaviour
{
    private const int MAX_PLAYER = 2;
    [Header("Host UI Elements")]
    [SerializeField] private TMP_InputField ipHostInputField;
    [SerializeField] private TMP_InputField portHostInputField;
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button stopHostButton;

    [Header("Connection UI Elements")]
    [SerializeField] private TextMeshProUGUI connectionStatusText;
    private bool isSceneLoading;

    [Header("Client UI Elements")]
    [SerializeField] private TMP_InputField ipClientInputField;
    [SerializeField] private TMP_InputField portClientInputField;
    [SerializeField] private Button startClientButton;


    private void Awake()
    {
        QualitySettings.vSyncCount = 0; // Disables VSync
        Application.targetFrameRate = 60; // Sets the target frame rate to 60 FPS

        SetupHostUI();

        SetupClientUI();
    }

    private void Start()
    {
        // Set up the connection approval callback and client connected callback
        if (NetworkManager.Singleton.ConnectionApprovalCallback == null)
            NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
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


    private void OnClientConnected(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsHost || isSceneLoading) return;

        if (NetworkManager.Singleton.ConnectedClients.Count == MAX_PLAYER)
        {
            isSceneLoading = true;
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
        stopHostButton.onClick.AddListener(OnStopHostButtonClicked);
    }

    private void OnStopHostButtonClicked()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.Shutdown();
            connectionStatusText.text = "Host stopped.";
        }
        else
        {
            connectionStatusText.text = "Host is not running.";
        }
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
                connectionStatusText.text = "Waiting for players...";
                NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoaded;
            }
            else
            {
                connectionStatusText.text = "Host is already running.";
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
            else
            {
                connectionStatusText.text = "Please stop host before starting again.";
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

    private void OnDisable()
    {
        startClientButton.onClick.RemoveAllListeners();
        startHostButton.onClick.RemoveAllListeners();
        stopHostButton.onClick.RemoveAllListeners();
    }
}
