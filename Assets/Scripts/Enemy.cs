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
        ReceiveDamage(other.GetComponent<Weapon>());
    }


    private void OnCollisionEnter(Collision other) 
    {
        ReceiveDamage(other.gameObject.GetComponent<Weapon>());
    }

    private void ReceiveDamage(Weapon weapon)
    {
        damageReceived = weapon.DamagePerShot;
        health -= damageReceived;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
