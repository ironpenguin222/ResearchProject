using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PoolManager : MonoBehaviour
{
    private Dictionary<string, Queue<GameObject>> pools = new Dictionary<string, Queue<GameObject>>(); // Pools for each type

    public GameObject Get(string type, GameObject prefab)
    {
        if (pools.TryGetValue(type, out Queue<GameObject> queue) && queue.Count > 0)
        {
            GameObject obj = queue.Dequeue(); // Gets pooled object
            obj.SetActive(true); // Reactivates object
            return obj;
        }

        return Instantiate(prefab); // Instantiates if none in pool
    }

    public void Return(string type, GameObject obj)
    {
        obj.SetActive(false); // Deactivates object

        if (!pools.ContainsKey(type)) 
            pools[type] = new Queue<GameObject>(); // Makes pool if none exists

        pools[type].Enqueue(obj); // Adds object to pool
    }
}
