using System.Collections.Generic;
using UnityEngine;

public class SimplePool<T> where T : Component
{
    readonly Queue<T> q = new Queue<T>();
    readonly T prefab;
    readonly Transform parent;

    public SimplePool(T prefab, int initial, Transform parent)
    {
        this.prefab = prefab; this.parent = parent;
        for (int i = 0; i < initial; i++)
        {
            var obj = GameObject.Instantiate(prefab, parent);
            obj.gameObject.SetActive(false);
            q.Enqueue(obj);
        }
    }

    public T Get()
    {
        var obj = q.Count > 0 ? q.Dequeue() : GameObject.Instantiate(prefab, parent);
        obj.gameObject.SetActive(true);
        return obj;
    }

    public void Return(T obj)
    {
        obj.gameObject.SetActive(false);
        q.Enqueue(obj);
    }
}
