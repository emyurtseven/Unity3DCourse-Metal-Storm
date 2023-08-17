using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    protected WeaponType weaponType;
    protected float damagePerShot;

    protected ParticleSystem particles;
    protected List<ParticleCollisionEvent> collisionEvents;

    protected Timer timer;

    // Damage per collision
    public float DamagePerShot { get => damagePerShot; set => damagePerShot = value; }
    public WeaponType Type { get => weaponType; set => weaponType = value; }

    protected virtual void Start()
    {
        if (this.weaponType != WeaponType.MachineGun)
        {
            timer = gameObject.AddComponent<Timer>();
            timer.Duration = 5f;
            timer.Run();
            StartCoroutine(ProjectileSelfDestruct());
        }
    }

    protected IEnumerator ProjectileSelfDestruct()
    {
        while (true)
        {
            if (timer.Finished)
            {
                Destroy(gameObject);
                yield break;
            }
            else
            {
                yield return new WaitForSeconds(1);
            }
        }
    }

    // On collisions will be implemented in children classes
    protected virtual void OnParticleCollision(GameObject other) { }
    protected virtual void OnCollisionEnter(Collision other) { }
}
