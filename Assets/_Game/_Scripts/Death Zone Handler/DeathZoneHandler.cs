using Unity.Netcode;
using UnityEngine;

public class DeathZoneHandler : NetworkBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) return;
        
        if (collision.gameObject.name.Contains("Player"))
        {
            PlayerDamageReceiver playerDamageReceiver = collision.gameObject.GetComponentInChildren<PlayerDamageReceiver>();
            if (playerDamageReceiver != null)
            {
                playerDamageReceiver.LostLife();
            }
            else
            {
                Debug.LogError("PlayerDamageReceiver component not found on the player object.");
            }
        }
    }
}
