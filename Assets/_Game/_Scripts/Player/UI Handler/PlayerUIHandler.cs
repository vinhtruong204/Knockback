using System;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIHandler : NetworkBehaviour
{
    private PlayerDamageReceiver playerDamageReceiver;
    private PlayerInputHandler playerInputHandler;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private Slider healthSlider;

    private NetworkVariable<FixedString64Bytes> playerName = new(
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    private void Awake()
    {
        playerInputHandler = transform.parent.GetComponentInChildren<PlayerInputHandler>();
        playerDamageReceiver = transform.parent.GetComponentInChildren<PlayerDamageReceiver>();
    }

    private void OnHealthChanged(int obj)
    {
        healthSlider.value = (float)obj / 100;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        if (IsOwner)
        {
            playerName.Value = $"Player{OwnerClientId}";
            playerNameText.text = playerName.Value.ToString();
        }
        else
        {
            playerNameText.text = playerName.Value.ToString();
        }

        playerDamageReceiver.HealthChanged += OnHealthChanged;

        // Subscribe to playerName changes
        playerName.OnValueChanged += OnPlayerNameChanged;

        // Subscribe to input events
        playerInputHandler.OnMove += OnMoveInput;
    }

    private void OnMoveInput(Vector2 vector)
    {
        if (vector.x > 0)
            transform.localScale = new Vector3(0.02f, 0.02f, 1f);
        else if (vector.x < 0)
            transform.localScale = new Vector3(-0.02f, 0.02f, 1f);
    }

    private void OnPlayerNameChanged(FixedString64Bytes previousValue, FixedString64Bytes newValue)
    {
        playerNameText.text = newValue.ToString();
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        // Unsubscribe from playerName changes
        playerName.OnValueChanged -= OnPlayerNameChanged;

        // Unsubscribe from input events
        playerInputHandler.OnMove -= OnMoveInput;

        // Unsubscribe from player events
        playerDamageReceiver.HealthChanged -= OnHealthChanged;
    }
}
