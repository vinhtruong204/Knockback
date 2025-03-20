using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Grenade : NetworkBehaviour, IDisableAfterTime
{
    private float timeToReturnPoolMax = 2f;
    private float force = 5f;
    private Rigidbody2D grenadeRigidbody;

    private void Awake()
    {
        grenadeRigidbody = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        // Add force to the grenade in the direction it is facing
        if (transform.localScale.x < 0f)
            grenadeRigidbody.AddForce(Vector2.left * force, ForceMode2D.Impulse);
        else if (transform.localScale.x > 0)
            grenadeRigidbody.AddForce(Vector2.right * force, ForceMode2D.Impulse);

        StartCoroutine(DisableAfterTime());
    }

    public IEnumerator DisableAfterTime()
    {
        yield return new WaitForSeconds(timeToReturnPoolMax);
        // ProjectilePoolManager.Instance.ReturnObject(PoolType.Grenade, this);
    }
}
