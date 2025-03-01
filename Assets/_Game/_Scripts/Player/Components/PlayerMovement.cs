using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PlayerMovement : MonoBehaviour
{
    public PlayerSettings _settings;
    
    private async void Awake()
    {
        (PlayerSettings settings, AsyncOperationHandle<PlayerSettings> handle) = await AddressableLoader<PlayerSettings>.LoadAssetAsync("PlayerSettings");
        
        _settings = settings;
        AddressableLoader<PlayerSettings>.ReleaseHandle(handle);
        
    }

}