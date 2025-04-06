using Unity.Netcode;
using UnityEngine;

public class NetworkManagerCheck : MonoBehaviour
{
    private void Awake()
    {
        if (FindObjectsByType<NetworkManager>(FindObjectsSortMode.None).Length > 1)
        {
            // Multiple NetworkManagers detected!
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject); // Giữ NetworkManager qua các scene
    }

}
