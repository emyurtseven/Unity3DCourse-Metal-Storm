using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] float health;
    [SerializeField] bool isPlayer;
    [SerializeField] GameObject explosionPrefab;

    float damageReceived;


    private void OnParticleCollision(GameObject other)
    {
        ReceiveDamage(other.GetComponent<Weapon>());
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.GetMask("Player"))
        {
            return;
        }
        else
        {
            ReceiveDamage(other.gameObject.transform.parent.GetComponent<Weapon>());
        }
    }

    private void ReceiveDamage(Weapon projectile)
    {
        damageReceived = projectile.DamagePerShot;
        health -= damageReceived;

        if (health <= 0)
        {
            HandleDeath(explosionPrefab);
        }
    }

    private void HandleDeath(GameObject deathEffect)
    {
        if (!isPlayer)
        {
            if (deathEffect != null)
            {
                Instantiate(deathEffect, transform.position, Quaternion.identity);
            }
            
            Destroy(gameObject);
        }
    }
}
