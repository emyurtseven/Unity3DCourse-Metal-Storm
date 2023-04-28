using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// Base script for game objects that need to shoot projectiles.
/// Note: A derived class EnemyShooter is attached to enemy game objects.
/// </summary>
public abstract class Shooter : MonoBehaviour
{
    [SerializeField] protected float cannonDamagePerShot;

    [Header("Missile Options")]
    [SerializeField] protected float missileDamagePerShot;
    [SerializeField] protected float missileMaxSpeed;
    [SerializeField] protected float missileRotationSpeed;
    [SerializeField] protected float armingDelay;
    [SerializeField] protected float focusDistance;

    [Header("Weapon and effect references")]
    [SerializeField] protected GameObject[] cannons;    // Children objects that have the colliding particle systems
    [SerializeField] protected GameObject missileLauncher;    // Children missile objects that will fly off
    [SerializeField] protected GameObject[] muzzleFlashes;        // Children muzzle flash objects

    [SerializeField] protected AudioSource cannonAudioSource;

    protected ParticleSystem[] cannonParticles;

    protected bool isFiringGun;

    public ParticleSystem[] CannonParticles { get => cannonParticles; set => cannonParticles = value; }


    protected abstract void Start();

    protected abstract void Update();

    /// <summary>
    ///  Get references to the colliding particle systems.
    /// </summary>
    protected void SetUpCannons()
    {
        CannonParticles = new ParticleSystem[cannons.Length];

        for (int i = 0; i < cannons.Length; i++)
        {
            CannonParticles[i] = cannons[i].GetComponent<ParticleSystem>();
            cannons[i].GetComponent<Weapon>().DamagePerShot = cannonDamagePerShot;
        }
    }

    protected void SetUpMissiles()
    {
        foreach (Transform item in missileLauncher.transform)
        {
            Missile missile = (Missile)(item.gameObject.GetComponent<Weapon>());

            missile.DamagePerShot = this.missileDamagePerShot;
            missile.MissileMaxSpeed = this.missileMaxSpeed;
            missile.MissileRotationSpeed = this.missileRotationSpeed;
            missile.ArmingDelay = this.armingDelay;
            missile.FocusDistance = this.focusDistance;
        }
    }

    /// <summary>
    /// Activates particle systems and muzzle flashes.
    /// Bool isFiringGun can be either from player input or in case of enemies, timed events.
    /// </summary>
    protected void FireCannon()
    {
        for (int i = 0; i < CannonParticles.Length; i++)
        {   
            // This logic block ensures correct emitting and stopping of particle systems
            if (CannonParticles[i].isEmitting)
            {
                if (!isFiringGun)
                {
                    CannonParticles[i].Stop();
                    muzzleFlashes[i].SetActive(false);
                }
            }
            else
            {
                if (isFiringGun)
                {
                    CannonParticles[i].Play();
                    muzzleFlashes[i].SetActive(true);
                }
            }
        }
    }

    protected void FireMissile()
    {
        
    }
}
