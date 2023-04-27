using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script deals only with impact effects; note that damage dealing is implemented in the "Health" script. 
/// Attach this to the object(s) which has the particle system component, to manage particle collisions.
/// </summary>
public class Cannon : Weapon
{
    protected override void OnParticleCollision(GameObject other)
    {
        GameObject bulletImpact;
        particles.GetCollisionEvents(other, collisionEvents);
        Vector3 collisionPos = collisionEvents[0].intersection;     // Get collision position

        // Convert object tag to tag + "Impact" so that it matches the PooledObjectType enum members
        string pooledObjectEnumStr = LayerMask.LayerToName(other.layer) + "Impact";     

        // Convert string into enum. This equates to the first enum value i.e. "None", if unsuccesfull
        Enum.TryParse(pooledObjectEnumStr, out PooledObjectType type);

        // Log warning if pooled object type is invalid
        if (type == PooledObjectType.None)
        {
            Debug.LogWarning(@"Impact effect does not exist. Check collided object layers. Spawning generic terrain impact instead.");
            type = PooledObjectType.TerrainImpact;
        }

        // Get the specified type from object pool and place it at the collision position
        bulletImpact = ObjectPool.GetPooledObject(type);
        bulletImpact.transform.position = collisionPos;
        bulletImpact.SetActive(true);
    }
}
