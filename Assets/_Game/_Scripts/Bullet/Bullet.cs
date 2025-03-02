using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;
    private Vector2 moveDirection;
    private float timeToReturnPool = 0f;

    void OnEnable()
    {
        moveDirection = transform.localScale.x < 0f ? Vector2.left : Vector2.right;
        timeToReturnPool = 0f;
    }

    private void Update()
    {
        transform.Translate(moveDirection * _speed * Time.deltaTime);

        timeToReturnPool += Time.deltaTime;
        
        if (timeToReturnPool >= 2f)
        {
            ProjectilePoolManager.Instance.ReturnObject("BulletPool", this);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            return;
        }

        if (collision.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.AddForce(moveDirection * 10f, ForceMode2D.Impulse);
            Destroy(collision.gameObject, 3f);
        }

        ProjectilePoolManager.Instance.ReturnObject("BulletPool", this);
    }

}
