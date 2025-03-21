using System;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIHandler : NetworkBehaviour
{
    private PlayerInputHandler playerInputHandler;
    [SerializeField] private TextMeshProUGUI playerNameText;

    [Header("Player Health And Heart UI")]
    private PlayerDamageReceiver playerDamageReceiver;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI heartText;

    [Header("Network Variables")]
    private NetworkVariable<FixedString64Bytes> playerName = new(
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    // The local scale x of the player
    private NetworkVariable<float> scaleX = new(
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    // TO DO: Update UI to display the health of the player and other player information

    private void Awake()
    {
        playerInputHandler = transform.parent.GetComponentInChildren<PlayerInputHandler>();
        playerDamageReceiver = transform.parent.GetComponentInChildren<PlayerDamageReceiver>();
    }

    private void OnHealthChanged(int currentHealth, int maxHealth)
    {
        healthSlider.value = (float)currentHealth / maxHealth;
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
            playerNameText.color = new Color(1f, 0.4f, 0.4f);
            OnHeartChanged(playerDamageReceiver.Heart);
        }

        // 
        playerDamageReceiver.HealthChanged += OnHealthChanged;
        playerDamageReceiver.HeartChanged += OnHeartChanged;

        // Subscribe to playerName changes
        playerName.OnValueChanged += OnPlayerNameChanged;

        // Subscribe to input events
        playerInputHandler.OnMove += OnMoveInput;

        // Subscribe to local scale x changes
        scaleX.OnValueChanged += OnScaleXChanged;
    }

    private void OnHeartChanged(int newHeart)
    {
        heartText.text = newHeart.ToString();
    }

    private void OnScaleXChanged(float previousValue, float newValue)
    {
        transform.localScale = new Vector3(newValue, transform.localScale.y, transform.localScale.z);
    }

    private void OnMoveInput(Vector2 vector)
    {
        if (vector.x > 0)
            transform.localScale = new Vector3(0.02f, 0.02f, 1f);
        else if (vector.x < 0)
            transform.localScale = new Vector3(-0.02f, 0.02f, 1f);

        // Only the owner can change the scale
        if (IsOwner)
            scaleX.Value = transform.localScale.x != scaleX.Value ? transform.localScale.x : scaleX.Value;
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

        // Unsubscribe from player damage receiver events
        playerDamageReceiver.HealthChanged -= OnHealthChanged;
        playerDamageReceiver.HeartChanged -= OnHeartChanged;

        // Unsubscribe from local scale x changes
        scaleX.OnValueChanged -= OnScaleXChanged;
    }
}
