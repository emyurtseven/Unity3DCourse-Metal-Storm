using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// Base script for game objects that need to shoot projectiles.
/// 
/// Note: A derived class EnemyShooter is attached to enemy game objects, 
/// and PlayerShooter is attached to player.
/// 
/// Individual values of weapons are set in this script and then applied to their classes, 
/// rather than setting it on the weapon itself.
/// </summary>
public abstract class Shooter : Invoker
{
    [SerializeField] GameObject missileAmmo;        // Missile prefab to instantiate
    [SerializeField] GameObject rocketAmmo;        // Missile prefab to instantiate
    [SerializeField] protected GameObject cannonAmmo;   // Cannon shell prefab to instantiate

    [Header("MG Options")]
    [SerializeField] protected float machineGunDamagePerShot;

    [Header("Projectile Options")]
    // Determines damage of any instantiated projectile except machinegun, which is a particle system
    [SerializeField] protected float projectileDamagePerShot;    
    [SerializeField] protected float projectileSpeed;
    [SerializeField] protected float missileRotationSpeed;

    // After this delay the missile will activate its collider and seek the target
    [SerializeField] protected float armingDelay;
    // The distance from target at which missile will stop seeking, so that it can be avoided
    [SerializeField] protected float focusDistance;

    [SerializeField] protected int rocketShotsCount;
    [SerializeField] protected float rocketShotsInterval;

    [Header("Weapon and effect references")]
    [SerializeField] protected GameObject[] machineGuns;    // Children objects that have the colliding particle systems
    [SerializeField] protected GameObject[] projectileSpawnPoints;    // Spawn point of cannon shells
    [SerializeField] protected GameObject[] muzzleFlashes;        // Children muzzle flash objects

    [SerializeField] protected AudioSource weaponAudioSource;   // On the firing object itself

    protected ParticleSystem[] machineGunParticles;      // Muzzle flash particles

    // Target object
    protected GameObject target;

    // Aiming position if a target object is not present
    protected Vector3 firingDirection;


    protected bool isFiring;    // This parameter is used to fire warious weapons
    protected bool gunWindingDown = true;   // Used for audio fade out


    public ParticleSystem[] MachineGunParticles { get => machineGunParticles; set => machineGunParticles = value; }

    protected abstract void Start();

    protected virtual void Update() { }

    /// <summary>
    ///  Get references to the colliding particle systems.
    /// </summary>
    protected void SetUpMachineGuns()
    {
        machineGunParticles = new ParticleSystem[machineGuns.Length];

        for (int i = 0; i < machineGuns.Length; i++)
        {
            MachineGunParticles[i] = machineGuns[i].GetComponent<ParticleSystem>();
            machineGuns[i].GetComponent<Weapon>().DamagePerShot = machineGunDamagePerShot;
        }
    }

    /// <summary>
    /// Apply settings to instantiated missile
    /// </summary>
    /// <param name="missile"></param>
    protected void SetUpMissile(Missile missile)
    {
        missile.DamagePerShot = this.projectileDamagePerShot;
        missile.MissileMaxSpeed = this.projectileSpeed;
        missile.MissileRotationSpeed = this.missileRotationSpeed;
        missile.ArmingDelay = this.armingDelay;
        missile.FocusDistance = this.focusDistance;
    }

    /// <summary>
    /// Activates particle systems and muzzle flashes.
    /// Bool isFiringGun can be either from player input or in case of enemies, timed events.
    /// </summary>
    protected void FireMachineGun()
    {
        for (int i = 0; i < MachineGunParticles.Length; i++)
        {   
            // This logic block ensures correct emitting and stopping of particle systems
            if (MachineGunParticles[i].isEmitting)
            {
                if (!isFiring)
                {
                    MachineGunParticles[i].Stop();
                    muzzleFlashes[i].SetActive(false);
                }
            }
            else
            {
                if (isFiring)
                {
                    MachineGunParticles[i].Play();
                    muzzleFlashes[i].SetActive(true);
                }
            }
        }
    }

    /// <summary>
    /// Instantiates and fires missile.
    /// </summary>
    /// <param name="targetCoordinates"></param>
    /// <param name="targetObject"></param>
    protected void FireMissile(Vector3? targetCoordinates = null, GameObject targetObject = null)
    {
        GameObject newMissile = Instantiate(missileAmmo, projectileSpawnPoints[0].transform.position, projectileSpawnPoints[0].transform.rotation);
        SetUpMissile(newMissile.GetComponent<Missile>());

        if (targetObject != null)
        {
            newMissile.GetComponent<Missile>().LaunchMissile(targetObject: targetObject);
        }

        if (targetCoordinates.HasValue)
        {
            newMissile.GetComponent<Missile>().LaunchMissile(targetCoordinates);
        }
    }

    /// <summary>
    /// Instaniates and fires a cannon shell.
    /// </summary>
    public void FireCannon()
    {
        muzzleFlashes[0].GetComponent<ParticleSystem>().Play();
        GameObject newShell = Instantiate(cannonAmmo, projectileSpawnPoints[0].transform.position, transform.rotation);
        CannonShell cannonShell = (CannonShell)(newShell.GetComponent<Weapon>());

        cannonShell.ShellSpeed = this.projectileSpeed;
        cannonShell.DamagePerShot = this.projectileDamagePerShot;

        // Get vector from shooter to target
        Vector3 targetPos = this.firingDirection;
        Vector3 direction = targetPos - projectileSpawnPoints[0].transform.position;

        // Normalize it to length 1
        direction.Normalize();

        newShell.transform.rotation = Quaternion.FromToRotation(newShell.transform.up, direction);

        // Set the velocity
        Rigidbody shellRigidbody = newShell.GetComponent<Rigidbody>();
        Vector3 velocity = direction * projectileSpeed;
        shellRigidbody.velocity = velocity;

        weaponAudioSource.Play();
    }

    /// <summary>
    /// Instaniates and fires a rocket.
    /// </summary>
    public IEnumerator FireMiniRockets(int waveCount, float waveInterval)
    {
        int rocketSpawnCount = projectileSpawnPoints.Length;

        // Get vector from shooter to target
        Vector3 targetPos = this.firingDirection;
        Vector3 direction = this.firingDirection - projectileSpawnPoints[0].transform.position;


        // Normalize it to length 1
        direction.Normalize();

        for (int wave = 0; wave < waveCount; wave++)
        {
            for (int i = 0; i < rocketSpawnCount; i++)
            {
                GameObject newRocket = Instantiate(rocketAmmo, projectileSpawnPoints[i].transform.position, transform.rotation);
                MiniRocket rocket = (MiniRocket)(newRocket.GetComponent<Weapon>());

                newRocket.transform.rotation = Quaternion.FromToRotation(newRocket.transform.up, direction);

                rocket.RocketSpeed = this.projectileSpeed;
                rocket.DamagePerShot = this.projectileDamagePerShot;

                // Set the velocity
                Rigidbody rocketRigidbody = newRocket.GetComponent<Rigidbody>();
                Vector3 velocity = direction * projectileSpeed;
                rocketRigidbody.velocity = velocity;
            }

            yield return new WaitForSeconds(waveInterval);
        }
    }
}
