using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExitButton : NetworkBehaviour
{
    [SerializeField] private Button exitButton;

    private void Awake()
    {
        exitButton = GetComponent<Button>();
        exitButton.onClick.AddListener(OnExitButtonClicked);
    }

    private void OnExitButtonClicked()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            LoadMenuScene();
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            RequestServerToLoadSceneRpc();
        }
    }

    [Rpc(SendTo.Server)]
    private void RequestServerToLoadSceneRpc()
    {
        LoadMenuScene();
    }

    private void LoadMenuScene()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnLoadComplete;
    }

    private void OnLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        NetworkManager.Singleton.Shutdown();
        NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnLoadComplete;
    }

    private void OnDisable()
    {
        exitButton.onClick.RemoveAllListeners();
    }
}
