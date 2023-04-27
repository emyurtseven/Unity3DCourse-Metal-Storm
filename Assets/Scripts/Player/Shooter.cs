using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// Base script for game objects that need to shoot projectiles.
/// Note: A derived class EnemyShooter is attached to enemy game objects.
/// </summary>
public class Shooter : MonoBehaviour
{
    [SerializeField] float cannonDamagePerShot;
    [SerializeField] float missileDamagePerShot;

    [SerializeField] protected GameObject[] cannons;    // Children objects that have the colliding particle systems
    [SerializeField] GameObject[] muzzleFlashes;        // Children muzzle flash objects

    [SerializeField] protected AudioSource cannonAudioSource;

    ParticleSystem[] cannonParticles;

    protected bool isFiringGun;
    bool gunWindingDown = true;

    public ParticleSystem[] CannonParticles { get => cannonParticles; set => cannonParticles = value; }

    protected virtual void Start()
    {
        SetUpCannons();
    }

    protected void Update()
    {
        RotatePlayerCannon();
        FireCannon();
        PlayerCannonAudio();
    }

    /// <summary>
    ///  Get references to the colliding particle systems.
    /// </summary>
    private void SetUpCannons()
    {
        CannonParticles = new ParticleSystem[cannons.Length];

        for (int i = 0; i < cannons.Length; i++)
        {
            CannonParticles[i] = cannons[i].GetComponent<ParticleSystem>();
            cannons[i].GetComponent<Weapon>().DamagePerShot = cannonDamagePerShot;
        }
    }

    /// <summary>
    /// Rotates the cannon attached to player aircraft to point at the mouse cursor.
    /// </summary>
    private void RotatePlayerCannon()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            cannons[0].transform.LookAt(hit.point);
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

    /// <summary>
    /// Loops minigun audio if player keeps shooting, skips to winding down bit if player stops.
    /// </summary>
    private void PlayerCannonAudio()
    {
        if (isFiringGun && gunWindingDown)
        {
            // Reset and start the clip when player presses fire
            cannonAudioSource.Stop();
            cannonAudioSource.Play();
            gunWindingDown = false;
        }
        else if (cannonAudioSource.isPlaying && !isFiringGun && !gunWindingDown)
        {
            // This here is the time in seconds in minigun audio clip where it winds down with distant echoes
            cannonAudioSource.time = 3.9f; 
            gunWindingDown = true;
        }
        else if (isFiringGun && cannonAudioSource.time >= 3 && !gunWindingDown)
        {
            // Loop from 1 to 3 sec (firing continuously) as long as player keeps shooting
            cannonAudioSource.time = 1f;    
        }
    }

    /// <summary>
    /// Gets player input from input system
    /// </summary>
    private void OnFireGun(InputValue value)
    {
        isFiringGun = value.isPressed;
    }
}
