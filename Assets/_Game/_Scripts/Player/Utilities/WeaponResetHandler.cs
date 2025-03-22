using Unity.Netcode;
using UnityEngine;

/// <summary>
/// This class handle reset gun ammo and grenade count when player revived
/// </summary>
public class WeaponResetHandler : NetworkBehaviour
{
    /// <summary>
    /// The transform of the right hand hold the gun
    /// </summary>
    [SerializeField] private Transform righthandTransform;
    [SerializeField] private PlayerDamageReceiver playerDamageReceiver;
    [SerializeField] private PlayerThrowGrenade playerThrowGrenade;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            playerDamageReceiver.HeartChanged += OnPlayerRevived;

            if (playerDamageReceiver == null)
            {
                playerDamageReceiver = transform.parent.GetComponentInChildren<PlayerDamageReceiver>();
            }

            if (playerThrowGrenade == null)
            {
                playerThrowGrenade = transform.parent.GetComponentInChildren<PlayerThrowGrenade>();
            }
        }
    }

        /// <summary>
        /// Called when the player's heart changed. This will reset gun ammo and grenade count.
        /// </summary>
        /// <param name="newHeart">The new heart value.</param>
    private void OnPlayerRevived(int newHeart)
    {
        // Reset gun ammo
        for (int i = 0; i < righthandTransform.childCount; i++)
        {
            WeaponBase child = righthandTransform.GetChild(i).GetComponent<WeaponBase>();

            if (child is not MeleeWeapon)
            {
                (child as PrimaryWeapon).ResetAmmo();
            }
        }

        // Reset grenade count
        playerThrowGrenade.ResetGrenadeCount();
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (IsOwner)
        {
            playerDamageReceiver.HeartChanged -= OnPlayerRevived;

        }
    }
}
