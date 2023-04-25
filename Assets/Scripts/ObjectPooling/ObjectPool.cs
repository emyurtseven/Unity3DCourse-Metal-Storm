using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Provides object pooling for bullets and enemies
/// </summary>
public class ObjectPool : MonoBehaviour
{
    static GameObject[] bulletImpactPrefabs;
    static Dictionary<PooledObjectName, List<GameObject>> pools;

    /// <summary>
    /// Initializes the pools
    /// </summary>
    public static void Initialize()
    {
        bulletImpactPrefabs = Resources.LoadAll<GameObject>("Prefabs/Effects/Impacts");
        // initialize dictionary
        pools = new Dictionary<PooledObjectName, List<GameObject>>();
        pools.Add(PooledObjectName.BulletImpact,
            new List<GameObject>(30));

        // fill pool
        for (int i = 0; i < pools[PooledObjectName.BulletImpact].Capacity; i++)
        {
            pools[PooledObjectName.BulletImpact].Add(GetNewObject(PooledObjectName.BulletImpact));
        }
    }

    /// <summary>
    /// Gets a bullet object from the pool
    /// </summary>
    /// <returns>bullet</returns>
    public static GameObject GetBulletImpact()
    {
        // replace code below with correct code
        return GetPooledObject(PooledObjectName.BulletImpact);
    }

    /// <summary>
    /// Gets a pooled object from the pool
    /// </summary>
    /// <returns>pooled object</returns>
    /// <param name="name">name of the pooled object to get</param>
    static GameObject GetPooledObject(PooledObjectName name)
    {
        List<GameObject> pool = pools[name];

        // check for available object in pool
        if (pool.Count > 0)
        {
            // remove object from pool and return (replace code below)
            int i = Random.Range(0, pool.Count);
            GameObject obj = pool[i];
            pool.RemoveAt(i);
            return obj;
        }
        else
        {
            // pool empty, so expand pool and return new object (replace code below)
            pool.Capacity++;
            return GetNewObject(name);
        }
    }

    /// <summary>
    /// Returns a pooled object to the pool
    /// </summary>
    /// <param name="name">name of pooled object</param>
    /// <param name="obj">object to return to pool</param>
    public static void ReturnPooledObject(PooledObjectName name,
        GameObject obj)
    {
        // add your code here
        obj.SetActive(false);

        pools[name].Add(obj);
    }

    /// <summary>
    /// Gets a new object
    /// </summary>
    /// <returns>new object</returns>
    static GameObject GetNewObject(PooledObjectName name)
    {
        GameObject obj;

        int i = Random.Range(0, bulletImpactPrefabs.Length);
        obj = Instantiate(bulletImpactPrefabs[i], GameObject.Find("ImpactEffectPool").transform);
        
        obj.SetActive(false);
        return obj;
    }

    /// <summary>
    /// Removes all the pooled objects from the object pools
    /// </summary>
    public static void EmptyPools()
    {
        // add your code here
        foreach (KeyValuePair<PooledObjectName, List<GameObject>> keyValuePair in pools)
        {
            keyValuePair.Value.Clear();
        }
    }


    /// <summary>
    /// Gets the current pool count for the given pooled object
    /// </summary>
    /// <param name="name">pooled object name</param>
    /// <returns>current pool count</returns>
    public int GetPoolCount(PooledObjectName name)
    {
        if (pools.ContainsKey(name))
        {
            return pools[name].Count;
        }
        else
        {
            // should never get here
            return -1;
        }
    }

    /// <summary>
    /// Gets the current pool capacity for the given pooled object
    /// </summary>
    /// <param name="name">pooled object name</param>
    /// <returns>current pool capacity</returns>
    public int GetPoolCapacity(PooledObjectName name)
    {
        if (pools.ContainsKey(name))
        {
            return pools[name].Capacity;
        }
        else
        {
            // should never get here
            return -1;
        }
    }
}
