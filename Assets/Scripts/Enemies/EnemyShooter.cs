using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : Shooter
{

    [SerializeField] float activeDuration;
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
        // else if (WeaponType == WeaponType.Missile)
        // {
        //     base.SetUpMissile();
        // }
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
        isFiring = false;

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
                base.PlayMachineGunAudio(0, 3.86f);
            }
            else if (weaponType == WeaponType.CannonShell)
            {
                if (isFiring)
                {
                    base.FireCannon();
                    PauseFiring();
                }
            }
            else if (WeaponType == WeaponType.Missile)
            {
                if (isFiring)
                {
                    base.FireMissile(targetObject: this.target);
                    PauseFiring();
                }
            }
        }
    }

    /// <summary>
    /// Checks every second if target has entered the activation range.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckTargetInRange()
    {
        while (true)
        {
            if (this.target == null)     // Break if target is destroyed
            {
                yield break;
            }

            float distance = Vector3.Distance(target.transform.position, transform.position);

            if (distance <= activationRange)
            {
                targetInRange = true;       // Activate weapons
                TakeAim();
                break;
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }
        }

        while (true)
        {
            if (this.target == null)     // Break if target is destroyed
            {
                yield break;
            }

            float distance = Vector3.Distance(target.transform.position, transform.position);

            if (distance > activationRange)
            {
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
    private void AlternateFiringSequence()
    {
        if (timer.Finished && !isFiring)
        {
            TakeAim();
        }
        else if (timer.Finished && isFiring)
        {
            PauseFiring();
        }
    }

    private void TakeAim()
    {
        timer.Duration = activeDuration;
        timer.Run();

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

    public void StartFiring()
    {
        isFiring = true;
    }

    private void PauseFiring()
    {
        timer.Duration = idleDuration;
        timer.Run();
        isFiring = false;
    }

    public void StopFiring()
    {
        turretAim.IsIdle = true;
        isFiring = false;
        timer.Stop();
    }

    /// <summary>
    /// Turns the weapon roughly towards where the target will be. 
    /// Not a precise calculation by design. Maybe can be improved for higher difficulty?
    /// </summary>
    private void AimAhead()
    {
        firingDirection = this.target.transform.TransformPoint(Vector3.forward * leadingShotDistance);
        firingDirection += (Vector3.up * leadingShotDistance * -Mathf.Sin(target.transform.parent.eulerAngles.x));

        turretAim.IsIdle = false;
        turretAim.IsAimed = false;
        turretAim.AimPosition = firingDirection;
    }

    private void AimExact()
    {
        firingDirection = trajectoryPredictor.PredictInterceptionPos(this.target, cannonShellSpeed);
        turretAim.AimPosition = firingDirection;
        turretAim.IsAimed = false;
        turretAim.IsIdle = false;
    }


}
