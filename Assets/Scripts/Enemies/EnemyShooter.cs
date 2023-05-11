using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : Shooter
{

    [SerializeField] float activeDuration;  // Firing time in seconds
    [SerializeField] float idleDuration;    // Idling time in seconds
    [Range(1, 300)]
    [SerializeField] float activationRange;     // Firing is activated if target is within this range
    [SerializeField] float leadingShotDistance;  // Used only if weapon is machine gun

    [SerializeField] bool isActive = true;      // An enemy starts the firing sequence if it's active

    [SerializeField] WeaponType weaponType;     // Weapon class the enemy has

    bool targetInRange;

    TurretAim turretAim = null;
    TrajectoryPredictor trajectoryPredictor;

    private IEnumerator firingSequenceCoroutine;

    public WeaponType WeaponType { get => weaponType; set => weaponType = value; }

    private void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player");

        if (WeaponType == WeaponType.MachineGun)
        {
            base.SetUpMachineGuns();
        }
        else if (WeaponType == WeaponType.CannonShell)
        {
            trajectoryPredictor = GetComponent<TrajectoryPredictor>();
            trajectoryPredictor.MaxRange = activationRange + 50;
        }    
    }

    protected override void Start() 
    {
        if (!isActive)
        {
            return;
        }
        
        turretAim = GetComponent<TurretAim>();
        isFiring = false;

        StartCoroutine(CheckTargetInRange());
    }

    /// <summary>
    /// Checks every 0.5 seconds if target has entered or exited active range.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckTargetInRange()
    {
        // Check if target has entered acitive range
        while (true)
        {
            if (this.target == null)     // Break if target is destroyed or missing
            {
                yield break;
            }

            float distance = Vector3.Distance(target.transform.position, transform.position);

            // Break from first loop if player is detected
            if (distance <= activationRange)
            {
                firingSequenceCoroutine = AlternateFiringSequence();
                StartCoroutine(firingSequenceCoroutine);
                break;
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }
        }

        // This time check continuously if player has left active range
        while (true)
        {
            if (this.target == null)     // Break if target is destroyed
            {
                yield break;
            }

            float distance = Vector3.Distance(target.transform.position, transform.position);
            float angle = Vector3.Angle(transform.position - target.transform.position, target.transform.forward);

            // Break and terminate coroutine if player is no longer in range
            if (distance > activationRange || angle > 90)
            {
                turretAim.IsIdle = true;
                StopCoroutine(firingSequenceCoroutine);
                StopFiring();
                yield break;
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    /// <summary>
    /// Starts and pauses firing, according to firingDuration and idleDuration variables
    /// </summary>
    private IEnumerator AlternateFiringSequence()
    {
        while (true)
        {
            TakeAim();

            yield return new WaitForSecondsRealtime(activeDuration);

            StopFiring();

            yield return new WaitForSecondsRealtime(idleDuration);
        }
    }

    public void StartFiring()
    {
        if (weaponType == WeaponType.MachineGun)
        {
            isFiring = true;
            base.FireMachineGun();
            weaponAudioSource.Play();
        }
        else if (weaponType == WeaponType.CannonShell)
        {
            base.FireCannon();
        }
        else if (WeaponType == WeaponType.Missile)
        {
            base.FireMissile(targetObject: this.target);
        }
    }

    public void StopFiring()
    {
        if (weaponType == WeaponType.MachineGun)
        {
            isFiring = false;
            base.FireMachineGun();
            weaponAudioSource.time = 3.9f;
            Invoke("ResetMachineGunAudio", 2f);
        }
    }

    /// <summary>
    /// Aims the weapon towards target.
    /// </summary>
    private void TakeAim()
    {
        if (target == null)
        {
            return;
        }
        if (weaponType == WeaponType.CannonShell)
        {
            AimExact();
        }
        else
        {
            AimAhead();
        }
    }

    /// <summary>
    /// Turns the weapon roughly towards where the target will be. 
    /// Not a precise calculation by design. Used by machine guns to open fire in front of player path
    /// </summary>
    private void AimAhead()
    {
        firingDirection = this.target.transform.TransformPoint(Vector3.forward * leadingShotDistance);
        firingDirection += (Vector3.up * leadingShotDistance * -Mathf.Sin(target.transform.parent.eulerAngles.x));

        turretAim.IsIdle = false;
        turretAim.IsAimed = false;
        turretAim.AimPosition = firingDirection;
    }

    /// <summary>
    /// Much more extensive algorithm for predicting the players possible future position.
    /// Used by single shot weapons.
    /// </summary>
    private void AimExact()
    {
        firingDirection = trajectoryPredictor.PredictInterceptionPos(this.target, cannonShellSpeed);

        turretAim.IsIdle = false;
        turretAim.IsAimed = false;
        turretAim.AimPosition = firingDirection;
    }

    private void ResetMachineGunAudio()
    {
        weaponAudioSource.Stop();
        weaponAudioSource.time = 0;
    }

    private void OnDrawGizmos() 
    {
        target = GameObject.FindGameObjectWithTag("Player");
        DrawUtilities.DrawSphereWithLabel(transform.position, target.transform.position,
                                            activationRange, Color.red, 12, "Activation Range");
    }
}
