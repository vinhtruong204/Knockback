using UnityEngine;

[CreateAssetMenu(fileName = "PlayerMovementConfigSO", menuName = "Scriptable Objects/PlayerMovementConfigSO")]
public class PlayerMovementConfigSO : ScriptableObject
{
    [Header("Movement")]
    public float moveSpeed = 10f;
    public float acceleration = 10f;
    public float maxSpeed = 20f;
    public float linearDamping = 1f;

    [Header("Jump")]
    public float jumpForce = 2f;
}
