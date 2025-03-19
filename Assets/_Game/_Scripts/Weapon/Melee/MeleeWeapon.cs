using UnityEngine;

public class MeleeWeapon : WeaponBase
{
    public int Damage { get; private set; }
    public float AttackSpeed { get; private set; }

    private void Awake()
    {
        Damage = 10;
        AttackSpeed = 1.0f;
    }

    public override void Attack()
    {
        Debug.Log(Name + " swings and deals " + Damage + " damage!");
    }

    public override bool CanAttack()
    {
        return true;
    }
}
