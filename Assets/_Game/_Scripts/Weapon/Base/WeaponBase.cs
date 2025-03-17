using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public string Name { get; protected set; }
    public WeaponType Type { get; protected set; }
    public float Weight { get; protected set; }
    
    public GameObject Model { get; private set; }

    public WeaponBase(string name, WeaponType type, float weight, GameObject model)
    {
        Name = name;
        Type = type;
        Weight = weight;
        Model = model;
    }

    public abstract void Attack();
}
