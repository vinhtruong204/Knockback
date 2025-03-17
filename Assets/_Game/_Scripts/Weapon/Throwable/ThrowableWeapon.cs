using UnityEngine;

public class ThrowableWeapon : WeaponBase, IThrowable
{
    private float explosionDelay;
    private float explosionRadius;
    private int damage;

    private void Awake()
    {
        explosionDelay = 2f;
        explosionRadius = 2f;
        damage = 10;
    }

    public override void Attack()
    {
        Throw();
    }

    public void Throw()
    {
        Debug.Log(Name + " is thrown!");
        Explode();
    }

    private void Explode()
    {
        Debug.Log(Name + " explodes after " + explosionDelay + " seconds, dealing " + damage + " damage in radius " + explosionRadius);
    }
}
