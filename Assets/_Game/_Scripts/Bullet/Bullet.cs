using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour, IDisableAfterTime
{
    [SerializeField] private float _speed = 10f;
    private Vector2 _moveDirection;
    private float _timeToReturnPoolMax = 2f;
    private float _damage = 10f;

    void OnEnable()
    {
        _moveDirection = transform.localScale.x < 0f ? Vector2.left : Vector2.right;

        StartCoroutine(DisableAfterTime());
    }

    public IEnumerator DisableAfterTime()
    {
        yield return new WaitForSeconds(_timeToReturnPoolMax);
        ProjectilePoolManager.Instance.ReturnObject(PoolType.Bullet, this);
    }

    private void Update()
    {
        transform.Translate(_moveDirection * _speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            return;
        }

        if (collision.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.AddForce(_moveDirection * _damage, ForceMode2D.Impulse);
        }

        ProjectilePoolManager.Instance.ReturnObject(PoolType.Bullet, this);
    }

}
