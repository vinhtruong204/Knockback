using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public string Name { get; protected set; }
    public WeaponType Type { get; protected set; }
    public float Weight { get; protected set; }

    public GameObject Model { get; private set; }

    protected void Init(WeaponData data)
    {
        Name = data.weaponName;
        Type = data.type;
        Weight = data.weight;
        Model = data.weaponPrefab;
    }

    public abstract void Attack();

    public abstract bool CanAttack();
}
