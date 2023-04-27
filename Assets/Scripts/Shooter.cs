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
    [SerializeField] protected float rocketDamagePerShot;
    [SerializeField] protected float rocketSpeed;

    [SerializeField] protected GameObject[] cannons;    // Children objects that have the colliding particle systems
    [SerializeField] protected GameObject rocketLauncher;    // Children missile objects that will fly off
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

    protected void SetUpRockets()
    {
        foreach (Transform item in rocketLauncher.transform)
        {
            Rocket rocket = (Rocket)(item.gameObject.GetComponent<Weapon>());

            rocket.DamagePerShot = this.rocketDamagePerShot;
            rocket.RocketSpeed = this.rocketSpeed;
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

    protected void FireRocket()
    {
        
    }
}
