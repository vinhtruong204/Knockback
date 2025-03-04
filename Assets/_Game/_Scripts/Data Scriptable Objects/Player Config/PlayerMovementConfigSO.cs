using UnityEngine;

[CreateAssetMenu(fileName = "PlayerMovementConfigSO", menuName = "Scriptable Objects/PlayerMovementConfigSO")]
public class PlayerMovementConfigSO : ScriptableObject
{
    [Header("Movement")]
    public float moveSpeed = 15f;
    public float acceleration = 10f;
    public float maxSpeed = 10f;
    public float linearDamping = 2f;
}
