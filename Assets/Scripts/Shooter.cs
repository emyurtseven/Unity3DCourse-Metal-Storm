using System;
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
    [SerializeField] GameObject missileAmmo;
    [SerializeField] protected GameObject cannonAmmo;

    [Header("MG Options")]
    [SerializeField] protected float machineGunDamagePerShot;

    [Header("Cannon Options")]
    [SerializeField] protected float cannonDamagePerShot;
    [SerializeField] protected float cannonShellSpeed;

    [Header("Missile Options")]
    [SerializeField] protected float missileDamagePerShot;
    [SerializeField] protected float missileMaxSpeed;
    [SerializeField] protected float missileRotationSpeed;
    [SerializeField] protected float armingDelay;
    [SerializeField] protected float focusDistance;

    [Header("Weapon and effect references")]
    [SerializeField] protected GameObject[] machineGuns;    // Children objects that have the colliding particle systems
    [SerializeField] protected GameObject missileLauncher;    // Children missile objects that will fly off
    [SerializeField] protected GameObject cannon;    // Children missile objects that will fly off
    [SerializeField] protected GameObject[] muzzleFlashes;        // Children muzzle flash objects

    [SerializeField] protected AudioSource weaponAudioSource;

    protected ParticleSystem[] machineGunParticles;

    protected GameObject target;

    protected Vector3 firingDirection;


    protected bool isFiring;
    protected bool gunWindingDown = true;


    public ParticleSystem[] MachineGunParticles { get => machineGunParticles; set => machineGunParticles = value; }


    protected abstract void Start();

    protected abstract void Update();

    /// <summary>
    ///  Get references to the colliding particle systems.
    /// </summary>
    protected void SetUpMachineGuns()
    {
        MachineGunParticles = new ParticleSystem[machineGuns.Length];

        for (int i = 0; i < machineGuns.Length; i++)
        {
            MachineGunParticles[i] = machineGuns[i].GetComponent<ParticleSystem>();
            machineGuns[i].GetComponent<Weapon>().DamagePerShot = machineGunDamagePerShot;
        }
    }

    protected void SetUpMissile(Missile missile)
    {
        missile.DamagePerShot = this.missileDamagePerShot;
        missile.MissileMaxSpeed = this.missileMaxSpeed;
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

    protected void FireMissile(Vector3? targetCoordinates = null, GameObject targetObject = null)
    {
        GameObject newMissile = Instantiate(missileAmmo, missileLauncher.transform.position, missileLauncher.transform.rotation);
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

    public void FireCannon()
    {
        muzzleFlashes[0].GetComponent<ParticleSystem>().Play();
        GameObject newShell = Instantiate(cannonAmmo, cannon.transform.position, transform.rotation);
        CannonShell cannonShell = (CannonShell)(newShell.GetComponent<Weapon>());

        cannonShell.ShellSpeed = this.cannonShellSpeed;
        cannonShell.DamagePerShot = this.cannonDamagePerShot;

        Vector3 targetPos = this.firingDirection;
        Vector3 fromEnemyToPlayer = targetPos - cannon.transform.position;

        // Normalize it to length 1
        fromEnemyToPlayer.Normalize();

        newShell.transform.rotation = Quaternion.FromToRotation(newShell.transform.up, fromEnemyToPlayer);

        // Set the speed to whatever you want:
        Rigidbody rb = newShell.GetComponent<Rigidbody>();
        Vector3 velocity = fromEnemyToPlayer * cannonShellSpeed;
        rb.velocity = velocity;

        weaponAudioSource.Play();
    }

    protected void PlayMachineGunAudio(float repeatTime, float fadeOutTime)
    {
        if (isFiring && gunWindingDown)
        {
            // Reset and start the clip when player presses fire
            weaponAudioSource.Stop();
            weaponAudioSource.Play();
            gunWindingDown = false;
        }
        else if (weaponAudioSource.isPlaying && !isFiring && !gunWindingDown)
        {
            // This here is the time in seconds in minigun audio clip where it winds down with distant echoes
            weaponAudioSource.time = fadeOutTime;
            gunWindingDown = true;
        }
        else if (isFiring && weaponAudioSource.time >= 2.93f && !gunWindingDown)
        {
            // Loop from 1 to 3 sec (firing continuously) as long as player keeps shooting
            weaponAudioSource.time = repeatTime;
        }
    }

}
