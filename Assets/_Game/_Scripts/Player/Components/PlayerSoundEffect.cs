using System;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerSoundEffect : NetworkBehaviour
{
    private AudioSource audioSource;

    [SerializeField] private AudioClip fireSound;

    [SerializeField] private PlayerShoot playerShoot;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        LoadAudioClipsAsync();

        if (playerShoot == null)
        {
            playerShoot = transform.parent.GetComponentInChildren<PlayerShoot>();
            playerShoot.OnShoot += PlayFireSoundClientRpc;
        }

    }

    [Rpc(SendTo.ClientsAndHost)]
    private void PlayFireSoundClientRpc()
    {
        if (fireSound != null)
        {
            audioSource.PlayOneShot(fireSound);
        }
        else
        {
            Debug.LogError("Fire sound not loaded.");
        }
    }

    private async void LoadAudioClipsAsync()
    {
        if (fireSound != null) return;
        
        // Load the fire sound effect from Addressables
        var (asset, handle) = await AddressableLoader<AudioClip>.LoadAssetAsync("ak-47-shot-sfx_85bpm");
        if (asset != null)
        {
            fireSound = asset;
        }
        else
        {
            Debug.LogError("Failed to load fire sound effect.");
        }
        AddressableLoader<AudioClip>.ReleaseHandle(handle);
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        if (playerShoot != null)
        {
            playerShoot.OnShoot -= PlayFireSoundClientRpc;
        }
    }
}
