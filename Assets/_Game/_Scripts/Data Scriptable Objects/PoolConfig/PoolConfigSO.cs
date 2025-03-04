using UnityEngine;

[CreateAssetMenu(fileName = "PoolConfigSO", menuName = "Scriptable Objects/PoolConfigSO")]
public class PoolConfigSO : ScriptableObject
{
    public MonoBehaviour prefab;
    public int poolSize;
}