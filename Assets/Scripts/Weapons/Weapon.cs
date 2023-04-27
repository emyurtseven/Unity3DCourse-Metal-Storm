using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    protected float damagePerShot;
    protected int impactEffectCount;
    protected ParticleSystem particles;
    protected List<ParticleCollisionEvent> collisionEvents;

    // Damage per collision
    public float DamagePerShot { get => damagePerShot; set => damagePerShot = value; }

    protected virtual void Start()
    {
        particles = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    // On collisions will be implemented in children classes
    protected virtual void OnParticleCollision(GameObject other) { }
    protected virtual void OnCollisionEnter(Collision other) { }
}
