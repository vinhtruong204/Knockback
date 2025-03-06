using System.Collections;
using UnityEngine;

public class Grenade : MonoBehaviour, IDisableAfterTime
{
    private float _timeToReturnPoolMax = 2f;
    private float _force = 5f;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Grenade"), true);
        
        // Add force to the grenade in the direction it is facing
        if (transform.localScale.x < 0f)
            _rb.AddForce(Vector2.left * _force, ForceMode2D.Impulse);
        else if (transform.localScale.x > 0)
            _rb.AddForce(Vector2.right * _force, ForceMode2D.Impulse);

        StartCoroutine(DisableAfterTime());
    }

    public IEnumerator DisableAfterTime()
    {
        yield return new WaitForSeconds(_timeToReturnPoolMax);
        ProjectilePoolManager.Instance.ReturnObject(PoolType.Grenade, this);
    }
}
