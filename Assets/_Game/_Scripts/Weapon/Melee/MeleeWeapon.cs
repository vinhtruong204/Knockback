using UnityEngine;

public class MeleeWeapon : WeaponBase
{
    public int Damage { get; private set; }
    public float AttackSpeed { get; private set; }

    public MeleeWeapon(string name, int damage, float attackSpeed, float weight, GameObject model)
        : base(name, WeaponType.Melee, weight, model)
    {
        Damage = damage;
        AttackSpeed = attackSpeed;
    }

    public override void Attack()
    {
        Debug.Log(Name + " swings and deals " + Damage + " damage!");
    }
}
