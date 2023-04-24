using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float health;

    float damageReceived;
    
    // Start is called before the first frame update
    void Start()
    {

    }


    private void OnParticleCollision(GameObject other)
    {
        ReceiveDamage(other.GetComponent<CannonProjectile>());
    }


    private void OnCollisionEnter(Collision other) 
    {
        if (other.gameObject.layer == LayerMask.GetMask("Player"))
        {
            return;
        }
        else
        {
            ReceiveDamage(other.gameObject.transform.parent.GetComponent<CannonProjectile>());
        }
    }

    private void ReceiveDamage(CannonProjectile projectile)
    {
        damageReceived = projectile.DamagePerShot;
        health -= damageReceived;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
