using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : Invoker
{
    [SerializeField] float maxHealth;
    [SerializeField] bool isPlayer;
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] GameObject prefracturedPrefab;

    float currentHealth;

    EnemyDestroyedEvent enemyDeadEvent = new EnemyDestroyedEvent();
    HealthChangedEvent healthChangedEvent = new HealthChangedEvent();
    PlayerDestroyedEvent playerDestroyedEvent = new PlayerDestroyedEvent();

    public float MaxHealth { get => maxHealth; }

    private void Awake() 
    {
        if (isPlayer)
        {
            singleFloatArgEventDict.Add(EventType.HealthChanged, healthChangedEvent);
            noArgEventDict.Add(EventType.PlayerDestroyed, playerDestroyedEvent);
            EventManager.AddFloatArgumentInvoker(this, EventType.HealthChanged);
            EventManager.AddNoArgumentInvoker(this, EventType.PlayerDestroyed);
        }
        else
        {
            gameObjectArgEventDict.Add(EventType.EnemyDestroyed, enemyDeadEvent);
            EventManager.AddGameObjectArgumentInvoker(this, EventType.EnemyDestroyed);
        }
    }

    private void Start() 
    {
        currentHealth = maxHealth;
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

        if (this.isPlayer && other.gameObject.tag == "Terrain")
        {
            ReceiveDamage(currentHealth);      // Instant death if player collides with terrain.
        }
    }

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
        if (isPlayer)
        {
            if (GameManager.Instance.DebugMode)
            {
                return;
            }

            currentHealth -= damage;
            InvokeSingleFloatArgEvent(EventType.HealthChanged, currentHealth);
        }
        else
        {
            currentHealth -= damage;
        }

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
        GameObject explosion;

        // Get the specified type from object pool and place it at the collision position
        explosion = ObjectPool.GetPooledObject(PooledObjectType.Effects_Explosions_Vehicle);
        explosion.transform.position = transform.position;
        explosion.SetActive(true);

        if (prefracturedPrefab != null) { Instantiate(prefracturedPrefab, transform.position, Quaternion.identity); }

        if (isPlayer)
        {
            GameManager.Instance.RestartLevel(gameObject);
            InvokeNoArgumentEvent(EventType.PlayerDestroyed);
        }
        else
        {
            StartCoroutine(DestroyEnemy());
        }
    }

    /// <summary>
    /// Makes the enemy object invisible until the particles it emitted are gone.
    /// This is done to prevent still airborne particles from disappearing with the destroyed object.
    /// </summary>
    private IEnumerator DestroyEnemy()
    {
        Transform slab = transform.Find("TurretSlab");
        if (slab != null)
        {
            slab.parent = null;
        }

        GetComponent<Collider>().enabled = false;

        InvokeGameObjectArgEvent(EventType.EnemyDestroyed, this.gameObject);

        foreach (MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>())
        {
            renderer.enabled = false;
        }

        EnemyShooter shooter;

        if (TryGetComponent<EnemyShooter>(out shooter))
        {
            shooter.StopFiring();

            if (shooter.WeaponType != WeaponType.MachineGun)
            {
                Destroy(gameObject);
                yield break;
            }

            WaitForSeconds delay = new WaitForSeconds(0.2f);
            
            while (shooter.MachineGunParticles[0].isPlaying)
            {
                yield return delay;   // Check every 0.2 secs if particles are gone
            }
        }

        Destroy(gameObject);
    }
}
