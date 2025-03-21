using System;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class is used to manage the player's weapons and magazine UI
/// </summary>
public class WeaponManager : NetworkBehaviour
{
    private NetworkVariable<WeaponType> currentWeaponType = new NetworkVariable<WeaponType>(
        WeaponType.Primary,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    // Position of the right hand containing the weapon
    [SerializeField] private Transform rightHand;
    public WeaponBase CurrentWeapon => rightHand.GetChild((int)currentWeaponType.Value).GetComponent<WeaponBase>();

    #region UI
    [Header("UI Weapon Magazine")]
    [SerializeField] private TMP_Dropdown dropdownChangeWeapon;
    [SerializeField] private TextMeshProUGUI textWeaponMagazine;

    [Header("UI Grenade Count")]
    [SerializeField] private PlayerThrowGrenade playerThrowGrenade;
    [SerializeField] private GameObject buttonThrowGrenade;

    #endregion

    private void Awake()
    {
        if (playerThrowGrenade == null)
            playerThrowGrenade = transform.parent.GetComponentInChildren<PlayerThrowGrenade>();
    }

    private void Start()
    {
        LoadRightHandTransform();

        if (IsOwner)
        {
            SubcribeEventAmmoChange();
            UpdateMagazineText();
        }

        EnableCurrentWeapon();
    }

    private void SubcribeEventAmmoChange()
    {
        for (int i = 0; i < rightHand.childCount; i++)
        {
            WeaponBase child = rightHand.GetChild(i).GetComponent<WeaponBase>();

            if (child is IReloadable)
            {
                (child as IReloadable).OnAmmoChanged += OnAmmoChanged;
            }
        }
    }

    private void OnAmmoChanged(int ammo, int totalAmmo)
    {
        textWeaponMagazine.text = $"{currentWeaponType.Value} {ammo} / {totalAmmo}";
    }

    private void LoadDropdownAndWeapoonText()
    {
        dropdownChangeWeapon = GameObject.Find("Dropdown Change Weapon").GetComponent<TMP_Dropdown>();

        if (dropdownChangeWeapon == null)
        {
            Debug.LogError("Dropdown not found!.");
            return;
        }

        if (IsOwner)
        {
            dropdownChangeWeapon.AddOptions(Enum.GetNames(typeof(WeaponType)).ToList());
            textWeaponMagazine = dropdownChangeWeapon.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        }
    }

    private void LoadRightHandTransform()
    {
        if (rightHand != null) return;

        Debug.LogWarning("Right hand transform not assigned! Loading right hand transform...");


        // Get the animation transform
        Transform animationTransform = transform.parent.Find("Animation");
        if (animationTransform == null)
        {
            Debug.LogError("Animation transform not found! Please assign the right hand transform in the inspector.");
            return;
        }

        // Get the right hand transform
        rightHand = animationTransform.Find("Hand").GetChild(1);

        if (rightHand == null)
        {
            Debug.LogError("Right hand transform not found! Please assign the right hand transform in the inspector.");
            return;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        LoadDropdownAndWeapoonText();

        LoadButtonThrowGrenade();

        if (IsOwner)
        {
            dropdownChangeWeapon.value = (int)currentWeaponType.Value;

            // Subscribe to dropdown change
            dropdownChangeWeapon.onValueChanged.AddListener(ChangeWeapon);

            // Subscribe to grenade throw
            playerThrowGrenade.OnThrowGrenade += ChangeGrenadeCountText;
        }

        // Subscribe to weapon change
        currentWeaponType.OnValueChanged += ChangeWeapon;
    }

    private void ChangeGrenadeCountText(int grenadeCount)
    {
        buttonThrowGrenade.GetComponentInChildren<TextMeshProUGUI>().text = "Throw: " + grenadeCount;
    }

    private void LoadButtonThrowGrenade()
    {
        buttonThrowGrenade = GameObject.Find("ThrowButton");

        if (buttonThrowGrenade == null)
        {
            Debug.LogError("Throw button not found! Please assign the throw button in the inspector.");
            return;
        }
    }

    private void EnableCurrentWeapon()
    {
        for (int i = 0; i < rightHand.childCount; i++)
        {
            rightHand.GetChild(i).gameObject.SetActive(false);
        }

        CurrentWeapon.gameObject.SetActive(true);
    }

    /// <summary>
    /// Change the current weapon on other player
    /// </summary>
    /// <param name="previousValue"></param>
    /// <param name="newValue"></param>
    private void ChangeWeapon(WeaponType previousValue, WeaponType newValue)
    {
        // Hide the previous weapon
        rightHand.GetChild((int)previousValue).gameObject.SetActive(false);

        if (IsOwner && currentWeaponType.Value != WeaponType.Melee)
        {
            UpdateMagazineText();
        }

        // Change the current weapon
        rightHand.GetChild((int)newValue).gameObject.SetActive(true);
    }

    private void UpdateMagazineText()
    {
        textWeaponMagazine.text = $"{currentWeaponType.Value} {(CurrentWeapon as PrimaryWeapon).Ammo} / {(CurrentWeapon as PrimaryWeapon).TotalAmmo}";
    }

    /// <summary>
    /// Change the current weapon on local player
    /// </summary>
    /// <param name="newValue"></param>
    private void ChangeWeapon(int newValue)
    {
        CurrentWeapon.gameObject.SetActive(false);

        currentWeaponType.Value = (WeaponType)newValue;

        CurrentWeapon.gameObject.SetActive(true);
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        // Unsubscribe
        dropdownChangeWeapon.onValueChanged.RemoveAllListeners();
        currentWeaponType.OnValueChanged -= ChangeWeapon;

        if (IsOwner)
        {
            playerThrowGrenade.OnThrowGrenade -= ChangeGrenadeCountText;

            // Unsubscribe weapon events
            for (int i = 0; i < rightHand.childCount; i++)
            {
                WeaponBase child = rightHand.GetChild(i).GetComponent<WeaponBase>();

                if (child is IReloadable)
                {
                    (child as IReloadable).OnAmmoChanged -= OnAmmoChanged;
                }
            }
        }
    }

}
