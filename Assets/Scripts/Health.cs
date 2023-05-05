using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] float maxHealth;
    [SerializeField] bool isPlayer;
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] GameObject prefracturedPrefab;

    float currentHealth;

    GameManager gameManager;

    PointsAddedEvent pointsAddedEvent = new PointsAddedEvent();

    private void Start() 
    {
        currentHealth = maxHealth;

        if (isPlayer)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
        else
        {
            EventManager.AddInvoker(this);
        }
    }

    public void AddPointsAddedEventListener(UnityAction<int> listener)
    {
        pointsAddedEvent.AddListener(listener);
    }

    /// <summary>
    /// Manage collision with particle system based weapons such as cannons.
    /// </summary>
    /// <param name="other"></param>
    private void OnParticleCollision(GameObject other)
    {
        float damage = CalculateDamage(other);
        ReceiveDamage(damage);
    }

    /// <summary>
    /// Manage collision with other objects.
    /// </summary>
    private void OnCollisionEnter(Collision other)
    {

        if (other.gameObject.tag == "Weapon")
        {
            float damage = CalculateDamage(other.gameObject);
            ReceiveDamage(damage);
        }
        else
        {
            ReceiveDamage(currentHealth);      // Instant death if object collides with terrain.
        }
    }

    // private void OnTriggerEnter(Collider other) 
    // {
    //     if (other.gameObject.tag == "Weapon")
    //     {
    //         float damage = CalculateDamage(other.gameObject);
    //         ReceiveDamage(damage);
    //     }
    // }

    /// <summary>
    /// Gets damage value from either the projectile object or the object which has colliding particle system.
    /// </summary>
    /// <returns> Damage value </returns>
    private float CalculateDamage(GameObject projectile)
    {
        float damageReceived = projectile.GetComponent<Weapon>().DamagePerShot;
        return damageReceived;
    }

    /// <summary>
    /// Substracts health, checks for death.
    /// </summary>
    /// <param name="damage"></param>
    private void ReceiveDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            HandleDeath();
        }
    }

    /// <summary>
    /// Spawns fractured prefab and explosion particle effects, if available.
    /// Restarts level if player, destroys object if otherwise.
    /// </summary>
    private void HandleDeath()
    {
        if (explosionPrefab != null) { Instantiate(explosionPrefab, transform.position, Quaternion.identity); }
        if (prefracturedPrefab != null) { Instantiate(prefracturedPrefab, transform.position, Quaternion.identity); }

        if (!isPlayer)
        {
            StartCoroutine(DestroyEnemy());
        }
        else
        {
            gameManager.RestartLevel(gameObject);
        }
    }

    /// <summary>
    /// Makes the enemy object invisible until the particles it emitted are gone.
    /// This is done to prevent still airborne particles from disappearing with the destroyed object.
    /// </summary>
    private IEnumerator DestroyEnemy()
    {
        EnemyShooter shooter = GetComponent<EnemyShooter>();
        GetComponent<Collider>().enabled = false;
        shooter.StopFiring();

        pointsAddedEvent.Invoke(1);

        foreach (MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>())
        {
            renderer.enabled = false;
        }

        if (shooter.WeaponType != WeaponType.MachineGun)
        {
            Destroy(gameObject);
            yield break;
        }

        while (true)
        {
            if (shooter.MachineGunParticles[0].isPlaying)
            {
                yield return new WaitForSeconds(0.5f);   // Check every 0.5 secs if particles are gone
            }
            else
            {
                Destroy(gameObject);
                break;
            }
        }
    }
}
