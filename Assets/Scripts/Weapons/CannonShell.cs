using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonShell : Weapon
{
    float shellSpeed;

    public float ShellSpeed { get => shellSpeed; set => shellSpeed = value; }

    // Start is called before the first frame update
    protected override void Start()
    {
        this.Type = WeaponType.CannonShell;
        base.Start();
    }

    protected override void OnCollisionEnter(Collision other)
    {
        GameObject impactExplosion;

        // Get the specified type from object pool and place it at the collision position
        impactExplosion = ObjectPool.GetPooledObject(PooledObjectType.Effects_Impacts_Missile);
        impactExplosion.transform.position = transform.position;
        impactExplosion.SetActive(true);

        Destroy(gameObject);
    }
}
