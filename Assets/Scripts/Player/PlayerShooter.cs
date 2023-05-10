using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// Base script for game objects that need to shoot projectiles.
/// Note: A derived class EnemyShooter is attached to enemy game objects.
/// </summary>
public class PlayerShooter : Shooter
{   
    [Range(0.1f, 1f)] 
    [SerializeField] float sphereCastRadius;
    [Range(1f, 100f)] 
    [SerializeField] float range;
    public LayerMask layerMask;

    protected override void Start() 
    {
        base.SetUpMachineGuns();
    }

    protected override void Update()
    {
        RotatePlayerCannon();
        base.FireMachineGun();
        PlayMachineGunAudio(1f, 3.9f);
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
            machineGuns[0].transform.LookAt(hit.point);
        }
    }

    /// <summary>
    /// Gets player input from input system
    /// </summary>
    private void OnFireGun(InputValue value)
    {
        isFiring = value.isPressed;
    }

    private void OnFireRocket(InputValue value)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.SphereCast(ray, sphereCastRadius, out hit);

        Vector3 targetCoordinates = hit.point;
        GameObject targetObject = hit.collider.gameObject;

        if (targetObject == null)
        {
            base.FireMissile(targetCoordinates: targetCoordinates);
        }
        else if(targetObject.transform.root.tag == "Enemy") 
        {
            base.FireMissile(targetObject: hit.collider.gameObject);
        }
        else
        {
            base.FireMissile(targetCoordinates: targetCoordinates);
        }
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

    // private void OnDrawGizmos()
    // {
    //     Gizmos.DrawWireSphere(transform.position, range);

    //     RaycastHit hit;

    //     if (Physics.SphereCast(transform.position, sphereCastRadius, transform.forward * range, out hit, range, layerMask))
    //     {
    //         Gizmos.color = Color.green;
    //         Vector3 sphereCastMidpoint = transform.position + (transform.forward * hit.distance);
    //         Gizmos.DrawWireSphere(sphereCastMidpoint, sphereCastRadius);
    //         Gizmos.DrawSphere(hit.point, 0.1f);
    //         Debug.DrawLine(transform.position, sphereCastMidpoint, Color.green);
    //     }
    //     else
    //     {
    //         Gizmos.color = Color.red;
    //         Vector3 sphereCastMidpoint = transform.position + (transform.forward * (range - sphereCastRadius));
    //         Gizmos.DrawWireSphere(sphereCastMidpoint, sphereCastRadius);
    //         Debug.DrawLine(transform.position, sphereCastMidpoint, Color.red);
    //     }
    // }

}
