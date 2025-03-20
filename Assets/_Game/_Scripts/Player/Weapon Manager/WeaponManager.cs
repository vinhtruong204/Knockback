using System;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class WeaponManager : NetworkBehaviour
{
    private NetworkVariable<WeaponType> currentWeaponType = new NetworkVariable<WeaponType>(
        WeaponType.Primary,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    // Position of the right hand containing the weapon
    [SerializeField] private Transform rightHand;
    public WeaponBase CurrentWeapon => rightHand.GetChild((int)currentWeaponType.Value).GetComponent<WeaponBase>();

    // UI
    [SerializeField] private TMP_Dropdown dropdownChangeWeapon;

    private void Start()
    {
        LoadRightHandTransform();

        EnableCurrentWeapon();
    }

    private void LoadDropdown()
    {
        dropdownChangeWeapon = GameObject.Find("Dropdown Change Weapon").GetComponent<TMP_Dropdown>();

        if (dropdownChangeWeapon == null)
        {
            Debug.LogError("Dropdown not found!.");
            return;
        }

        if (IsOwner)
            dropdownChangeWeapon.AddOptions(Enum.GetNames(typeof(WeaponType)).ToList());
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

        LoadDropdown();

        if (IsOwner)
        {
            dropdownChangeWeapon.value = (int)currentWeaponType.Value;

            // Subscribe to dropdown change
            dropdownChangeWeapon.onValueChanged.AddListener(ChangeWeapon);
        }

        // Subscribe to weapon change
        currentWeaponType.OnValueChanged += ChangeWeapon;
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

        // Change the current weapon
        rightHand.GetChild((int)newValue).gameObject.SetActive(true);
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
    }

}
