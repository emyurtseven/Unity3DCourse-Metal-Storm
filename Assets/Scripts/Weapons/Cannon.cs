using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attach this 

public class Cannon : Weapon
{

    protected override void OnParticleCollision(GameObject other)
    {
        particles.GetCollisionEvents(other, collisionEvents);
        Vector3 collisionPos = collisionEvents[0].intersection;
        
        if (other.tag == "Terrain")
        {
            GameObject bulletImpact = ObjectPool.GetBulletImpact();
            bulletImpact.transform.position = collisionPos;
            bulletImpact.SetActive(true);
        }
    }
}
