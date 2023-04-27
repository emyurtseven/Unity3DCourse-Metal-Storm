using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An enumeration of pooled object names. 
/// Make sure the impact effects are named as gameObject.tag + Impact
/// </summary>
public enum PooledObjectType 
{
    None,
    TerrainImpact,
    VehicleImpact
}
