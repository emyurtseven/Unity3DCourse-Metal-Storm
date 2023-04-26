using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attach this 

public class Cannon : Weapon
{

    protected override void OnParticleCollision(GameObject other)
    {
        GameObject bulletImpact;
        particles.GetCollisionEvents(other, collisionEvents);
        Vector3 collisionPos = collisionEvents[0].intersection;
        
        if (other.tag == "Terrain")
        {
            bulletImpact = ObjectPool.GetPooledObject(PooledObjectType.TerrainImpact);
            bulletImpact.transform.position = collisionPos;
            bulletImpact.SetActive(true);
        }
        else
        {
            bulletImpact = ObjectPool.GetPooledObject(PooledObjectType.VehicleImpact);
            bulletImpact.transform.position = collisionPos;
            bulletImpact.SetActive(true);
        }
    }
}
