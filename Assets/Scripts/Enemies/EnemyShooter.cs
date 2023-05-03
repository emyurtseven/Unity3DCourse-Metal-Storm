using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : Shooter
{

    [SerializeField] float firingDuration;
    [SerializeField] float idleDuration;
    [SerializeField] float activationRange;     // Firing is activated if target is closer than this value
    [SerializeField] float leadingShotDistance;  // Used only if weapon is machine gun

    [SerializeField] bool isActive = true;

    [SerializeField] WeaponType weaponType;

    bool targetInRange;

    TurretAim turretAim = null;
    TrajectoryPredictor trajectoryPredictor;

    Timer timer;


    public WeaponType WeaponType { get => weaponType; set => weaponType = value; }

    private void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player");

        if (WeaponType == WeaponType.MachineGun)
        {
            base.SetUpMachineGuns();
        }
        else if (WeaponType == WeaponType.Missile)
        {
            base.SetUpMissiles();
        }
        else if (WeaponType == WeaponType.CannonShell)
        {
            trajectoryPredictor = GetComponent<TrajectoryPredictor>();
            trajectoryPredictor.MaxRange = activationRange + 50;
        }    
    }

    protected override void Start() 
    {
        turretAim = GetComponent<TurretAim>();
        timer = gameObject.AddComponent<Timer>();       // Add a timer component for alternating fire
        isFiringGun = false;

        StartCoroutine(CheckTargetInRange());
    }

    protected override void Update() 
    {
        if (targetInRange && isActive)
        {
            AlternateFiringSequence();

            if (weaponType == WeaponType.MachineGun)
            {
                base.FireMachineGun();
                EnemyMachineGunAudio();
            }
            else if (weaponType == WeaponType.CannonShell)
            {

            }
        }
    }

    /// <summary>
    /// Checks every 0.5f seconds if target has entered the activation range.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckTargetInRange()
    {
        while (true)
        {
            if (this.target == null)     // Break if target is destroyed
            {
                break;
            }

            float distance = Vector3.Distance(target.transform.position, transform.position);

            if (distance <= activationRange)
            {
                targetInRange = true;       // Activate weapons
                StartFiring();
                break;
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
    private void AlternateFiringSequence()
    {
        if (timer.Finished && !isFiringGun)
        {
            StartFiring();
        }
        else if (timer.Finished && isFiringGun)
        {
            PauseFiring();
        }
    }

    private void StartFiring()
    {
        timer.Duration = firingDuration;
        timer.Run();
        isFiringGun = true;

        if (weaponType == WeaponType.MachineGun)
        {
            AimAhead();
        }
        else if (weaponType == WeaponType.CannonShell)
        {
            AimTurret();

            // Invoke("FireCannon", 1f);
        }
        else if (WeaponType == WeaponType.Missile)
        {
            base.FireMissile(targetObject: this.target);
        }
    }

    private void AimTurret()
    {
        firingDirection = trajectoryPredictor.PredictInterceptionPos(this.target, cannonShellSpeed);
        turretAim.AimPosition = firingDirection;
        turretAim.IsAimed = false;
        turretAim.IsIdle = false;
    }

    private void PauseFiring()
    {
        timer.Duration = idleDuration;
        timer.Run();
        isFiringGun = false;
    }

    public void StopFiring()
    {
        turretAim.IsIdle = true;
        timer.Stop();
        isFiringGun = false;
    }

    /// <summary>
    /// Turns the weapon roughly towards where the target will be. 
    /// Not a precise calculation by design. Maybe can be improved for higher difficulty?
    /// </summary>
    private void AimAhead()
    {
        if (target == null)
        {
            return;
        }

        firingDirection = this.target.transform.TransformPoint(Vector3.forward * leadingShotDistance);
        firingDirection += (Vector3.up * leadingShotDistance * -Mathf.Sin(target.transform.parent.eulerAngles.x));
    }

    private void EnemyMachineGunAudio()
    {
        if (isFiringGun && !weaponAudioSource.isPlaying)
        {
            weaponAudioSource.Play();
        }
        else if (!isFiringGun && weaponAudioSource.isPlaying)
        {
            weaponAudioSource.Stop();
        }
    }


}
