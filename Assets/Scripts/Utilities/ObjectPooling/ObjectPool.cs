using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Provides object pooling for bullets and enemies
/// </summary>
public static class ObjectPool
{
    static Dictionary<PooledObjectType, GameObject[]> pooledPrefabs;
    static Dictionary<PooledObjectType, List<GameObject>> objectPools;

    /// <summary>
    /// Initializes the pools
    /// </summary>
    public static void Initialize()
    {
        GameObject objectPoolsHolder = new GameObject("ObjectPools");

        foreach (string name in Enum.GetNames(typeof(PooledObjectType)))
        {
            GameObject child = new GameObject(name);
            child.transform.parent = objectPoolsHolder.transform;
        }

        // initialize prefab dictionaries
        pooledPrefabs = new Dictionary<PooledObjectType, GameObject[]>();

        pooledPrefabs.Add(PooledObjectType.TerrainImpact, Resources.LoadAll<GameObject>("Prefabs/Effects/Impacts/Terrain"));
        pooledPrefabs.Add(PooledObjectType.SolidImpact, Resources.LoadAll<GameObject>("Prefabs/Effects/Impacts/Solid"));
        pooledPrefabs.Add(PooledObjectType.MissileExplosion, Resources.LoadAll<GameObject>("Prefabs/Effects/Impacts/Missile"));
        pooledPrefabs.Add(PooledObjectType.MiniRocketExplosion, Resources.LoadAll<GameObject>("Prefabs/Effects/Impacts/MiniRocket"));
        pooledPrefabs.Add(PooledObjectType.VehicleExplosion, Resources.LoadAll<GameObject>("Prefabs/Effects/Explosions"));
        
        // initialize pool dictionaries
        objectPools = new Dictionary<PooledObjectType, List<GameObject>>();

        objectPools.Add(PooledObjectType.TerrainImpact, new List<GameObject>(40));
        objectPools.Add(PooledObjectType.SolidImpact, new List<GameObject>(30));
        objectPools.Add(PooledObjectType.MissileExplosion, new List<GameObject>(10));
        objectPools.Add(PooledObjectType.MiniRocketExplosion, new List<GameObject>(20));
        objectPools.Add(PooledObjectType.VehicleExplosion, new List<GameObject>(10));

        // fill pools and create parent objects in scene
        foreach (PooledObjectType objectType in Enum.GetValues(typeof(PooledObjectType)))
        {
            for (int i = 0; i < objectPools[objectType].Capacity; i++)
            {
                objectPools[objectType].Add(GetNewObject(objectType));
            }
        }
    }

    /// <summary>
    /// Gets a pooled object from the pool
    /// </summary>
    /// <returns>pooled object</returns>
    /// <param name="name">name of the pooled object to get</param>
    public static GameObject GetPooledObject(PooledObjectType name)
    {
        if (!objectPools.ContainsKey(name))
        {
            Debug.LogError($"Key {name} is missing from the object pool");
            GameObject placeholder = GameObject.CreatePrimitive(PrimitiveType.Cube);
            return placeholder;
        }

        List<GameObject> pool = objectPools[name];

        // check for available object in pool
        if (pool.Count > 0)
        {
            // remove object from pool and return (replace code below)
            int i = UnityEngine.Random.Range(0, pool.Count);
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
    public static bool ReturnPooledObject(PooledObjectType name,
        GameObject obj)
    {
        if (!objectPools.ContainsKey(name))
        {
            return false;
        }
        
        obj.SetActive(false);
        objectPools[name].Add(obj);

        return true;
    }

    /// <summary>
    /// Gets a new object
    /// </summary>
    /// <returns>new object</returns>
    static GameObject GetNewObject(PooledObjectType type)
    {
        GameObject obj;

        int i = UnityEngine.Random.Range(0, pooledPrefabs[type].Length);

        obj = GameObject.Instantiate(pooledPrefabs[type][i], GameObject.Find(type.ToString()).transform);
        obj.GetComponent<ParticleEffectAutoDestroy>().Type = type;

        obj.SetActive(false);
        return obj;
    }

    /// <summary>
    /// Removes all the pooled objects from the object pools
    /// </summary>
    public static void EmptyPools()
    {
        // add your code here
        foreach (KeyValuePair<PooledObjectType, List<GameObject>> keyValuePair in objectPools)
        {
            keyValuePair.Value.Clear();
        }
    }


    /// <summary>
    /// Gets the current pool count for the given pooled object
    /// </summary>
    /// <param name="type">pooled object name</param>
    /// <returns>current pool count</returns>
    public static int GetPoolCount(PooledObjectType type)
    {
        if (objectPools.ContainsKey(type))
        {
            return objectPools[type].Count;
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
    public static int GetPoolCapacity(PooledObjectType name)
    {
        if (objectPools.ContainsKey(name))
        {
            return objectPools[name].Capacity;
        }
        else
        {
            // should never get here
            return -1;
        }
    }
}
