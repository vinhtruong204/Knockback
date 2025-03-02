using UnityEngine;

[CreateAssetMenu(fileName = "PlayerJumpConfigSO", menuName = "Scriptable Objects/PlayerJumpConfigSO")]
public class PlayerJumpConfigSO : ScriptableObject
{
    [Header("Jump")]
    public float jumpForce = 30f;
    public int maxJumps = 2;
    public float gravityScale = 1f;
}
