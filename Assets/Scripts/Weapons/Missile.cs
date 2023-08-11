using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : Weapon
{
    float missileMaxSpeed = 10f;
    float missileRotationSpeed = 1f;
    float armingDelay = 1f;
    float focusDistance = 10;

    bool isFired;

    Vector3 targetCoordinates;
    GameObject targetObject;

    Rigidbody myRigidbody;
    AudioClip explosionSfx;

    VelocityReader targetVelocityReader;

    bool hasTargetLock = false;

    public float MissileMaxSpeed { get => missileMaxSpeed; set => missileMaxSpeed = value; }
    public float ArmingDelay { get => armingDelay; set => armingDelay = value; }
    public float MissileRotationSpeed { get => missileRotationSpeed; set => missileRotationSpeed = value; }
    public float FocusDistance { get => focusDistance; set => focusDistance = value; }


    protected override void Start() 
    {
        int i = Random.Range(2, 5);
        string explosionName = $"missile_explosion_outdoors ({i})";
        explosionSfx = (AudioClip)Resources.Load("Audio/" + explosionName);
        this.Type = WeaponType.Missile;
        myRigidbody = GetComponent<Rigidbody>();

        if (targetObject != null)
        {
            targetVelocityReader = targetObject.GetComponent<VelocityReader>();
        }

        base.Start();
    }

    private void Update() 
    {
        if (isFired)
        {
            UpdateTargetPosition();
            AccelerateMissile();
        }
    }

    private void UpdateTargetPosition()
    {
        if (this.targetObject != null && hasTargetLock)
        {
            this.targetCoordinates = targetObject.transform.position;

            if (targetVelocityReader != null)
            {
                float timeToReachTarget = Vector3.Distance(transform.position, targetCoordinates) / missileMaxSpeed;
                this.targetCoordinates += (targetVelocityReader.AverageVelocity * timeToReachTarget);
            }
        }
    }

    private void AccelerateMissile()
    {
        Vector3 targetDirection = targetCoordinates - transform.position;

        myRigidbody.velocity = transform.forward * missileMaxSpeed;

        if (Vector3.Distance(transform.position, targetCoordinates) < FocusDistance)
        {
            hasTargetLock = false;
        }
        if (hasTargetLock)
        {
            var rotation = Quaternion.LookRotation(targetDirection);
            myRigidbody.MoveRotation(Quaternion.RotateTowards(transform.rotation, rotation, MissileRotationSpeed));
        }
    }

    public void LaunchMissile(Vector3? targetCoordinates=null, GameObject targetObject=null)
    {
        if (targetObject != null)
        {
            this.targetObject = targetObject;
        }

        if (targetCoordinates.HasValue)
        {
            this.targetCoordinates = targetCoordinates.Value;
        }

        gameObject.transform.parent = null;
        GetComponent<AudioSource>().Play();
        GetComponent<ParticleSystem>().Play();
        isFired = true;

        Invoke("ArmMissile", ArmingDelay);
    }

    private void ArmMissile()
    {
        GetComponent<Collider>().enabled = true;
        hasTargetLock = true;
    }

    protected override void OnCollisionEnter(Collision other) 
    {
        GameObject impactExplosion;

        // Get the specified type from object pool and place it at the collision position
        impactExplosion = ObjectPool.GetPooledObject(PooledObjectType.MissileExplosion);
        impactExplosion.transform.position = transform.position;
        impactExplosion.GetComponent<AudioSource>().clip = explosionSfx;
        impactExplosion.SetActive(true);

        Destroy(gameObject);
    }

    // private void OnTriggerEnter(Collider other) 
    // {
    //     GameObject impactExplosion;

    //     // Get the specified type from object pool and place it at the collision position
    //     impactExplosion = ObjectPool.GetPooledObject(PooledObjectType.MissileExplosion);
    //     impactExplosion.transform.position = transform.position;
    //     impactExplosion.SetActive(true);

    //     Destroy(gameObject);
    // }
}
