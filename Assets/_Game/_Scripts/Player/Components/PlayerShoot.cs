using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            _ = ProjectilePoolManager.Instance.GetObject<Bullet>("BulletPool", transform.parent);

        }
    }
}
