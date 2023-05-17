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
    [SerializeField] float overheatTime = 2f;
    [SerializeField] float overheatCooldown = 3f;

    [SerializeField] int startingMissileCount = 5;

    int currentMissileCount;

    float temperature = 0;
    bool gunOverheated = false;

    float gunHeatRate;
    float gunCoolRate = 75f;

    PlayerInput playerInput;

    SingleIntArgumentEvent missileFired = new SingleIntArgumentEvent();

    LayerMask layerMask;

    public float Temperature { get => temperature; }

    private void Awake() 
    {
        singleIntArgEventDict.Add(EventType.MissileFired, missileFired);
        EventManager.AddIntArgumentInvoker(this, EventType.MissileFired);
    }

    protected override void Start() 
    {
        currentMissileCount = startingMissileCount;
        gunHeatRate = 100 / overheatTime;
        playerInput = GetComponent<PlayerInput>();
        base.SetUpMachineGuns();

        // Get a layermask that includes everything EXCEPT the "Player" and "Ignore Raycast" layers, 
        // to be used by Physics.Raycast() to ignore player
        layerMask = ~LayerMask.GetMask("Player", "Ignore Raycast");

        InvokeSingleIntArgEvent(EventType.MissileFired, startingMissileCount);
    }

    protected override void Update()
    {
        isFiring = playerInput.actions["FireGun"].IsPressed() && !gunOverheated;

        RotatePlayerCannon();
        base.FireMachineGun();
        PlayMachineGunAudio(1f, 3.9f);
        
        if (!gunOverheated)
        {
            ManageMachineGunTemperature();
        }
    }

    private void ManageMachineGunTemperature()
    {
        if (isFiring)
        {
            temperature += (gunHeatRate * Time.deltaTime);

            if (temperature >= 100)
            {
                temperature = 100;
                StartCoroutine(OverheatMachinegun());
            }
        }
        else
        {
            temperature -= (gunCoolRate * Time.deltaTime);

            if (temperature <= 0)
            {
                temperature = 0;
            }
        }
    }

    private IEnumerator OverheatMachinegun()
    {
        gunOverheated = true;
        isFiring = false;

        yield return new WaitForSeconds(overheatCooldown);

        gunOverheated = false;
    }

    /// <summary>
    /// Rotates the cannon attached to player aircraft to point at the mouse cursor.
    /// </summary>
    private void RotatePlayerCannon()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, float.MaxValue, layerMask))
        {
            machineGuns[0].transform.LookAt(hit.point);
        }
    }

    // /// <summary>
    // /// Gets player input from input system
    // /// </summary>
    // private void OnFireGun(InputValue value)
    // {
    //     isFiring = value.isPressed;
    // }

    /// <summary>
    /// Called when assigned input is pressed. Raycasts to mouse pos and acquires a target.
    /// </summary>
    /// <param name="value"></param>
    private void OnFireMissile(InputValue value)
    {
        if (currentMissileCount == 0)
        {
            return;
        }

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.SphereCast(ray, sphereCastRadius, out hit);

        Vector3 targetCoordinates = hit.point;
        GameObject targetObject = hit.collider.gameObject;

        if (targetObject == null)
        {
            base.FireMissile(targetCoordinates: targetCoordinates);
        }
        else if(targetObject.transform.tag == "Enemy") 
        {
            base.FireMissile(targetObject: hit.collider.gameObject);
        }
        else
        {
            base.FireMissile(targetCoordinates: targetCoordinates);
        }

        currentMissileCount--;
        InvokeSingleIntArgEvent(EventType.MissileFired, currentMissileCount);
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
