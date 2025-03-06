using System;
using UnityEngine;

public class CheckOnGround : MonoBehaviour
{
    public static event Action OnGrounded;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            OnGrounded?.Invoke();
        }
    }
}
