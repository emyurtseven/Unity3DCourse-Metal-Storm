using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected float damagePerShot;

    protected int impactEffectCount;

    public float DamagePerShot { get => damagePerShot; }

    protected ParticleSystem particles;
    protected List<ParticleCollisionEvent> collisionEvents;

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
