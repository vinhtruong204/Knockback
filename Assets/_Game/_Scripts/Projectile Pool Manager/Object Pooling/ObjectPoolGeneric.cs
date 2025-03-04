using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolGeneric<T> where T : MonoBehaviour
{
    private Queue<T> _pool;
    private T _prefab;
    private int _initialSize;
    private Transform _parent;

    public ObjectPoolGeneric(T prefab, int initialSize, Transform parent)
    {
        // Initialize variables
        _prefab = prefab;
        _initialSize = initialSize;
        _parent = parent;
        _pool = new();

        // Create pool with initial size
        for (int i = 0; i < _initialSize; i++)
        {
            T obj = CreateObject();
            _pool.Enqueue(obj);
        }
    }

    private T CreateObject()
    {
        T obj = Object.Instantiate(_prefab);
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(_parent);
        return obj;
    }

    /// <summary>
    /// Get an object from the pool.
    /// </summary>
    /// <param name="spawnTransform">Spawn player at specific position.</param>
    /// <returns>Return object from pool.</returns>
    public T GetObject(Transform spawnTransform)
    {
        T obj;

        if (_pool.Count == 0)
        {
            obj = CreateObject();
        }
        else
        {
            obj = _pool.Dequeue();
        }

        obj.transform.SetPositionAndRotation(spawnTransform.position, spawnTransform.rotation);
        obj.transform.localScale = spawnTransform.localScale;
        obj.gameObject.SetActive(true);

        return obj;
    }

    /// <summary>
    /// Return object to the pool.
    /// </summary>
    /// <param name="obj">Object to return.</param>
    public void ReturnObject(T obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(_parent);
        _pool.Enqueue(obj);
    }
}
