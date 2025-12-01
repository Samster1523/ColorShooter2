using System.Collections.Generic;
using UnityEngine;

public class SimplePool<T> where T : Component
{
    readonly T _prefab;
    readonly Transform _parent;
    readonly Queue<T> _pool = new Queue<T>();

    public SimplePool(T prefab, int initialSize, Transform parent = null)
    {
        _prefab = prefab;
        _parent = parent;

        for (int i = 0; i < initialSize; i++)
        {
            var obj = Object.Instantiate(_prefab, _parent);
            obj.gameObject.SetActive(false);
            _pool.Enqueue(obj);
        }
    }

    public T Get()
    {
        if (_pool.Count > 0)
        {
            var obj = _pool.Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        }

        return Object.Instantiate(_prefab, _parent);
    }

    public void Return(T obj)
    {
        if (obj == null) return;
        obj.gameObject.SetActive(false);
        _pool.Enqueue(obj);
    }
}
