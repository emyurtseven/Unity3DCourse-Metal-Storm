using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : Shooter
{
    [SerializeField] float firingDuration;
    [SerializeField] float idleDuration;
    [SerializeField] float activationRange;     // Firing is activated if target is closer than this value
    [SerializeField] float leadingShotDistance;
    [SerializeField] bool isActive = true;

    [SerializeField] GameObject turret;
    [SerializeField] WeaponType weaponType;

    bool targetInRange;

    GameObject target;

    Vector3 lastPos = Vector3.zero;

    Timer timer;

    public WeaponType WeaponType { get => weaponType; set => weaponType = value; }

    protected override void Start() 
    {
        if (WeaponType == WeaponType.Cannon)
        {
            base.SetUpCannons();
        }
        else if (WeaponType == WeaponType.Missile)
        {
            base.SetUpMissiles();
        }

        target = GameObject.FindGameObjectWithTag("Player");
        timer = gameObject.AddComponent<Timer>();       // Add a timer component for alternating fire
        isFiringGun = false;

        StartCoroutine(CheckTargetInRange());
    }

    protected override void Update() 
    {
        if (targetInRange && isActive)
        {
            AlternateFiringSequence();

            if (WeaponType == WeaponType.Cannon)
            {
                base.FireCannon();
                EnemyCannonAudio();
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
            if (target == null)     // Break if target is destroyed
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
        TakeAim();
        timer.Duration = firingDuration;
        timer.Run();
        isFiringGun = true;

        if (WeaponType == WeaponType.Missile)
        {
            base.FireMissile(targetObject: target);
        }
    }

    private void PauseFiring()
    {
        timer.Duration = idleDuration;
        timer.Run();
        isFiringGun = false;
    }

    public void StopFiring()
    {
        timer.Stop();
        isFiringGun = false;
    }

    /// <summary>
    /// Turns the weapon roughly towards where the target will be. 
    /// Not a precise calculation by design. Maybe can be improved for higher difficulty?
    /// </summary>
    private void TakeAim()
    {
        if (target == null)
        {
            return;
        }

        Vector3 targetLeadingPosition = target.transform.TransformPoint(Vector3.forward * leadingShotDistance);
        targetLeadingPosition += (Vector3.up * leadingShotDistance * -Mathf.Sin(target.transform.parent.eulerAngles.x));

        turret.transform.LookAt(targetLeadingPosition);
    }

    private void EnemyCannonAudio()
    {
        if (isFiringGun && !cannonAudioSource.isPlaying)
        {
            cannonAudioSource.Play();
        }
        else if (!isFiringGun && cannonAudioSource.isPlaying)
        {
            cannonAudioSource.Stop();
        }
    }
}
