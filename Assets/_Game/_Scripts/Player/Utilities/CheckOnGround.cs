using System;
using UnityEngine;

public class CheckOnGround : MonoBehaviour
{
    public static CheckOnGround LocalInstance { get; private set; }
    public event Action OnGrounded;

    private void Awake()
    {
        if (LocalInstance != null && LocalInstance != this)
        {
            Destroy(gameObject);
            return;
        }

        LocalInstance = this;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            OnGrounded?.Invoke();
        }
    }
}
