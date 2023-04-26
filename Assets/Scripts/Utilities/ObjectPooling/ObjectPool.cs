using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Provides object pooling for bullets and enemies
/// </summary>
public class ObjectPool : MonoBehaviour
{
    static Dictionary<PooledObjectType, GameObject[]> impactPrefabs;

    static Dictionary<PooledObjectType, List<GameObject>> impactEffectPools;

    /// <summary>
    /// Initializes the pools
    /// </summary>
    public static void Initialize()
    {
        // initialize prefab dictionary
        impactPrefabs = new Dictionary<PooledObjectType, GameObject[]>();

        impactPrefabs.Add(PooledObjectType.TerrainImpact, Resources.LoadAll<GameObject>("Prefabs/Effects/Impacts/Terrain"));
        impactPrefabs.Add(PooledObjectType.VehicleImpact, Resources.LoadAll<GameObject>("Prefabs/Effects/Impacts/Vehicle"));
        
        // initialize pool dictionary
        impactEffectPools = new Dictionary<PooledObjectType, List<GameObject>>();

        impactEffectPools.Add(PooledObjectType.TerrainImpact, new List<GameObject>(30));
        impactEffectPools.Add(PooledObjectType.VehicleImpact, new List<GameObject>(30));

        // fill pools
        for (int i = 0; i < impactEffectPools[PooledObjectType.TerrainImpact].Capacity; i++)
        {
            impactEffectPools[PooledObjectType.TerrainImpact].Add(GetNewObject(PooledObjectType.TerrainImpact));
        }
        for (int i = 0; i < impactEffectPools[PooledObjectType.VehicleImpact].Capacity; i++)
        {
            impactEffectPools[PooledObjectType.VehicleImpact].Add(GetNewObject(PooledObjectType.VehicleImpact));
        }
    }

    /// <summary>
    /// Gets a pooled object from the pool
    /// </summary>
    /// <returns>pooled object</returns>
    /// <param name="name">name of the pooled object to get</param>
    public static GameObject GetPooledObject(PooledObjectType name)
    {
        List<GameObject> pool = impactEffectPools[name];

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
    public static void ReturnPooledObject(PooledObjectType name,
        GameObject obj)
    {
        // add your code here
        obj.SetActive(false);

        impactEffectPools[name].Add(obj);
    }

    /// <summary>
    /// Gets a new object
    /// </summary>
    /// <returns>new object</returns>
    static GameObject GetNewObject(PooledObjectType type)
    {
        GameObject obj;

        int i = Random.Range(0, impactPrefabs[type].Length);

        obj = Instantiate(impactPrefabs[type][i], GameObject.Find(type.ToString()).transform);
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
        foreach (KeyValuePair<PooledObjectType, List<GameObject>> keyValuePair in impactEffectPools)
        {
            keyValuePair.Value.Clear();
        }
    }


    /// <summary>
    /// Gets the current pool count for the given pooled object
    /// </summary>
    /// <param name="type">pooled object name</param>
    /// <returns>current pool count</returns>
    public int GetPoolCount(PooledObjectType type)
    {
        if (impactEffectPools.ContainsKey(type))
        {
            return impactEffectPools[type].Count;
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
    public int GetPoolCapacity(PooledObjectType name)
    {
        if (impactEffectPools.ContainsKey(name))
        {
            return impactEffectPools[name].Capacity;
        }
        else
        {
            // should never get here
            return -1;
        }
    }
}
