using System;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using UnityEngine;
using UnityEngine.UI;

public class HandleLANConnection : MonoBehaviour
{
    [SerializeField] private TMP_InputField ipInputField;
    [SerializeField] private TMP_InputField portInputField;
    [SerializeField] private Button connectButton;

    private const int MAX_PLAYER = 2;

    private void Start()
    {
        connectButton.onClick.AddListener(OnConnectButtonClicked);

        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
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
            response.CreatePlayerObject = true;
            response.Pending = false;
        }
    }

    private void OnConnectButtonClicked()
    {
        string ip = ipInputField.text;
        string port = portInputField.text;

        if (string.IsNullOrEmpty(ip) || string.IsNullOrEmpty(port))
        {
            Debug.LogError("IP or Port cannot be empty.");
            return;
        }

        NetworkManager networkManager = NetworkManager.Singleton;
        if (networkManager != null)
        {
            networkManager.GetComponent<UnityTransport>().SetConnectionData(ip, ushort.Parse(port));
            networkManager.StartClient();
        }
        else
        {
            Debug.LogError("NetworkManager not found in the scene.");
        }
    }

    private void OnDestroy()
    {
        connectButton.onClick.RemoveListener(OnConnectButtonClicked);
    }
}
