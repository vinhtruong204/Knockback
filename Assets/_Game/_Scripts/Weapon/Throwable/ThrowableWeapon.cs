using UnityEngine;

public class ThrowableWeapon : WeaponBase, IThrowable
{
    private float explosionDelay;
    private float explosionRadius;
    private int damage;

    public ThrowableWeapon(string name, float explosionDelay, float explosionRadius, int damage, float weight, GameObject model)
        : base(name, WeaponType.Throwable, weight, model)
    {
        this.explosionDelay = explosionDelay;
        this.explosionRadius = explosionRadius;
        this.damage = damage;
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
