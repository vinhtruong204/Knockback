using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ProjectilePoolManager : MonoBehaviour
{
    // Singleton
    public static ProjectilePoolManager Instance { get; private set; }

    // Dictionary to store all the pools
    private Dictionary<PoolType, ObjectPoolGeneric<MonoBehaviour>> poolsDictionary;

    private async void Awake()
    {
        Instance = this;
        poolsDictionary = new Dictionary<PoolType, ObjectPoolGeneric<MonoBehaviour>>();

        await SetupProjectilePools();
    }

    /// <summary>
    /// Setup the pools.
    /// </summary>
    /// <returns></returns>
    private async Task SetupProjectilePools()
    {
        // Create a pool for each projectile type
        foreach (PoolType poolType in System.Enum.GetValues(typeof(PoolType)))
        {
            // Load the PoolConfigSO for the projectile type
            string poolConfigAddress = poolType.ToString();
            var (asset, handle) = await AddressableLoader<PoolConfigSO>.LoadAssetAsync(poolConfigAddress);

            if (asset != null)
            {
                GetTransformToSpawn(poolType, out Transform spawnTransform);

                // Create the pool
                ObjectPoolGeneric<MonoBehaviour> pool = new(asset.prefab, asset.poolSize, spawnTransform);
                poolsDictionary.Add(poolType, pool);
            }
            else
            {
                Debug.LogWarning("PoolConfigSO for " + poolType + " not found.");
            }
            
            // Release the handle
            AddressableLoader<PoolConfigSO>.ReleaseHandle(handle);
        }
    }

    /// <summary>
    /// Get the transform to spawn the projectile.
    /// </summary>
    /// <param name="poolType">Type of pool.</param>
    /// <param name="spawnTransform">Position in hierachy to spawn.</param>
    private void GetTransformToSpawn(PoolType poolType, out Transform spawnTransform)
    {
        spawnTransform = transform.Find(poolType.ToString() + "Pool");

        if (spawnTransform == null)
        {
            Debug.LogWarning("Transform to spawn " + poolType + " not found. Creating a new one.");
            spawnTransform = new GameObject(poolType.ToString() + "Pool").transform;
            spawnTransform.SetParent(transform);
        }
    }

    /// <summary>
    /// Get an object from the pool.
    /// </summary>
    /// <typeparam name="T">Type of T is Monobehavior.</typeparam>
    /// <param name="poolType">Type of pool want to get object.</param>
    /// <param name="transform">Reset position with new transform.</param>
    /// <returns>Object is returned from pool.</returns>
    public T GetObject<T>(PoolType poolType, Transform transform) where T : MonoBehaviour
    {
        if (!poolsDictionary.ContainsKey(poolType))
        {
            Debug.LogWarning("Pool with name " + poolType + " doesn't exist.");
            return null;
        }

        ObjectPoolGeneric<MonoBehaviour> pool = poolsDictionary[poolType];
        MonoBehaviour obj = pool.GetObject(transform);
        return obj as T;
    }

    /// <summary>
    /// Return an object to the pool.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="poolType"></param>
    /// <param name="obj"></param>
    public void ReturnObject<T>(PoolType poolType, T obj) where T : MonoBehaviour
    {
        if (!poolsDictionary.ContainsKey(poolType))
        {
            Debug.LogWarning("Pool with name " + poolType + " doesn't exist.");
            return;
        }

        poolsDictionary[poolType].ReturnObject(obj);
    }
}
