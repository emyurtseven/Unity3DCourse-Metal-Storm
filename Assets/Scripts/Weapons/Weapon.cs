using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    private WeaponType type;
    protected float damagePerShot;

    protected ParticleSystem particles;
    protected List<ParticleCollisionEvent> collisionEvents;

    // Damage per collision
    public float DamagePerShot { get => damagePerShot; set => damagePerShot = value; }
    public WeaponType Type { get => type; set => type = value; }

    protected abstract void Start();

    // On collisions will be implemented in children classes
    protected virtual void OnParticleCollision(GameObject other) { }
    protected virtual void OnCollisionEnter(Collision other) { }
}
