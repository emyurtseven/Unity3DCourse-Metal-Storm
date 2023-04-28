using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : Shooter
{
    [SerializeField] float firingDuration;
    [SerializeField] float idleDuration;
    [SerializeField] float activationRange;     // Firing is activated if player is closer than this value
    [SerializeField] float leadingShotDistance;
    [SerializeField] bool isActive = true;

    [SerializeField] GameObject turret;

    [SerializeField] WeaponType weaponType;

    bool playerInRange;

    GameObject player;

    Timer timer;

    protected override void Start() 
    {
        if (weaponType == WeaponType.Cannon)
        {
            base.SetUpCannons();
        }
        else if (weaponType == WeaponType.Missile)
        {
            base.SetUpMissiles();
        }

        player = GameObject.FindGameObjectWithTag("Player");
        timer = gameObject.AddComponent<Timer>();       // Add a timer component for alternating fire
        isFiringGun = false;

        StartCoroutine(CheckPlayerInRange());
    }

    protected override void Update() 
    {
        if (playerInRange && isActive)
        {
            if (weaponType == WeaponType.Cannon)
            {
                AlternateFiringSequence();       // Enemies only
                base.FireCannon();
                EnemyCannonAudio();
            }
            else if (weaponType == WeaponType.Missile)
            {
                return;
            }
        }
    }

    /// <summary>
    /// Checks every 0.5f seconds if player has entered the activation range.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckPlayerInRange()
    {
        while (true)
        {
            if (player == null)     // Break if player is destroyed
            {
                break;
            }

            float distance = Vector3.Distance(player.transform.position, transform.position);

            if (distance <= activationRange)
            {
                playerInRange = true;       // Activate weapons
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
    /// Turns the weapon roughly towards where the player will be. 
    /// Not a precise calculation by design. Maybe can be improved for higher difficulty?
    /// </summary>
    private void TakeAim()
    {
        Vector3 playerLeadingPosition = player.transform.TransformPoint(Vector3.forward * leadingShotDistance);
        playerLeadingPosition += (Vector3.up * leadingShotDistance * -Mathf.Sin(player.transform.parent.eulerAngles.x));

        turret.transform.LookAt(playerLeadingPosition);
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
