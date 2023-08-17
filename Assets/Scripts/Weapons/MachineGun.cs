using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script deals only with impact effects; note that damage dealing is implemented in the "Health" script. 
/// Attach this to the object(s) which has the particle system component, to manage particle collisions.
/// </summary>
public class MachineGun : Weapon
{
    protected override void Start()
    {
        this.Type = WeaponType.MachineGun;
        particles = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }


    protected override void OnParticleCollision(GameObject other)
    {
        GameObject bulletImpact;
        particles.GetCollisionEvents(other, collisionEvents);
        Vector3 collisionPos = collisionEvents[0].intersection;     // Get collision position

        string pooledObjectEnumStr;

        // For layers "Player" and "Vehicle", SolidImpact is used
        if (other.layer == LayerMask.NameToLayer("Player") || 
                other.layer == LayerMask.NameToLayer("Vehicle") ||
                other.layer == LayerMask.NameToLayer("Debris"))
        {
            pooledObjectEnumStr = "Effects_Impacts_Solid";
        }
        else
        {
            // Convert object layer to layer + "Impact" so that it matches the PooledObjectType enum members
            pooledObjectEnumStr = "Effects_Impacts_" + LayerMask.LayerToName(other.layer);
        }

        // Convert string into enum. This equates to the first enum value i.e. "None", if unsuccesfull
        if (Enum.TryParse(pooledObjectEnumStr, out PooledObjectType type))
        {
            // Get the specified type from object pool and place it at the collision position
            bulletImpact = ObjectPool.GetPooledObject(type);
            bulletImpact.transform.position = collisionPos;
            bulletImpact.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"Impact effect for {pooledObjectEnumStr} does not exist. Check collided object layers. Spawning generic terrain impact instead.");
            type = PooledObjectType.Effects_Impacts_Terrain;
        }
    }
}
