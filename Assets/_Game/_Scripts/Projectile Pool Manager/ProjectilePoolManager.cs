using System.Collections.Generic;
using UnityEngine;

public class ProjectilePoolManager : MonoBehaviour
{
    public static ProjectilePoolManager Instance { get; private set; }

    [System.Serializable]
    public class PoolConfig
    {
        public MonoBehaviour prefab;
        public int poolSize;
        public string poolName;
    }

    public List<PoolConfig> poolConfigs;
    private Dictionary<string, ObjectPoolGeneric<MonoBehaviour>> poolsDictionary;

    private void Awake()
    {
        Instance = this;
        poolsDictionary = new Dictionary<string, ObjectPoolGeneric<MonoBehaviour>>();

        SetupProjectilePools();
    }

    private void SetupProjectilePools()
    {
        foreach (PoolConfig config in poolConfigs)
        {
            string poolName = string.IsNullOrEmpty(config.poolName) ? config.prefab.name + "Pool" : config.poolName;
            Transform spawnParent = transform.Find(poolName);

            // If the parent doesn't exist, create a new one
            if (spawnParent == null)
            {
                spawnParent = new GameObject(poolName).transform;
                spawnParent.SetParent(transform);
            }

            ObjectPoolGeneric<MonoBehaviour> pool = new(config.prefab, config.poolSize, spawnParent);
            poolsDictionary.Add(poolName, pool);
        }
    }

    public T GetObject<T>(string poolName, Transform transform) where T : MonoBehaviour
    {
        if (!poolsDictionary.ContainsKey(poolName))
        {
            Debug.LogWarning("Pool with name " + poolName + " doesn't exist.");
            return null;
        }

        ObjectPoolGeneric<MonoBehaviour> pool = poolsDictionary[poolName];
        MonoBehaviour obj = pool.GetObject(transform);
        return obj as T;
    }

    public void ReturnObject<T>(string poolName, T obj) where T : MonoBehaviour
    {
        if (!poolsDictionary.ContainsKey(poolName))
        {
            Debug.LogWarning("Pool with name " + poolName + " doesn't exist.");
            return;
        }

        poolsDictionary[poolName].ReturnObject(obj);
    }
}
