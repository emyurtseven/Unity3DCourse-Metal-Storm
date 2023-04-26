using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    private float damagePerShot;

    protected int impactEffectCount;

    protected ParticleSystem particles;
    protected List<ParticleCollisionEvent> collisionEvents;

    public float DamagePerShot { get => damagePerShot; set => damagePerShot = value; }

    protected virtual void Start()
    {
        particles = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }


    protected abstract void OnParticleCollision(GameObject other);

    protected virtual void OnCollisionEnter(Collision other) 
    {
        return;
    }
}
