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
    bool gunWindingDown = true;

    protected override void Start() 
    {
        base.SetUpCannons();
        base.SetUpMissiles();
    }

    protected override void Update()
    {
        RotatePlayerCannon();
        base.FireCannon();
        PlayerCannonAudio();
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

    private void OnFireRocket(InputValue value)
    {
        if (missileLauncher.transform.childCount > 0)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out hit);
            Vector3 target = hit.point;

            missileLauncher.transform.GetChild(0).GetComponent<Missile>().LaunchMissile(target);
        }
    }
}
