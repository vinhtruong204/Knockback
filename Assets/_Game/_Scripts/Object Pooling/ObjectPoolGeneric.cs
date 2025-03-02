using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolGeneric<T> where T : MonoBehaviour
{
    private Queue<T> _pool;
    private T _prefab;
    private int _initialSize;
    private Transform _parent;

    public ObjectPoolGeneric(T prefab, int initialSize, Transform parent = null)
    {
        _prefab = prefab;
        _initialSize = initialSize;
        _parent = parent != null ? parent : new GameObject(typeof(T).Name + "Pool").transform;

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
        T obj = UnityEngine.Object.Instantiate(_prefab);
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(_parent);
        return obj;
    }

    public T GetObject(Vector2 position, Quaternion rotation)
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

        obj.gameObject.SetActive(true);
        obj.transform.SetPositionAndRotation(position, rotation);
        
        return obj;
    }

    public void ReturnObject(T obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(_parent);
        _pool.Enqueue(obj);
    }
}
