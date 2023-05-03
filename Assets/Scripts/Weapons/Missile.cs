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

    bool isLookingAtObject = false;

    public float MissileMaxSpeed { get => missileMaxSpeed; set => missileMaxSpeed = value; }
    public float ArmingDelay { get => armingDelay; set => armingDelay = value; }
    public float MissileRotationSpeed { get => missileRotationSpeed; set => missileRotationSpeed = value; }
    public float FocusDistance { get => focusDistance; set => focusDistance = value; }


    protected override void Start() 
    {
        this.Type = WeaponType.Missile;
        myRigidbody = GetComponent<Rigidbody>();
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
        if (this.targetObject != null)
        {
            float offset = targetObject.GetComponent<Collider>().bounds.extents.z;
            
            this.targetCoordinates = targetObject.transform.position +
                                        targetObject.transform.forward * offset;
        }
    }

    private void AccelerateMissile()
    {
        Vector3 targetDirection = targetCoordinates - transform.position;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection,
                                                        MissileRotationSpeed * Time.deltaTime, 0);

        transform.Translate(Vector3.forward * Time.deltaTime * missileMaxSpeed, Space.Self);

        if (Vector3.Distance(transform.position, targetCoordinates) < FocusDistance)
        {
            isLookingAtObject = false;
        }

        if (isLookingAtObject)
        {
            transform.rotation = Quaternion.LookRotation(newDirection);
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
        isLookingAtObject = true;
    }

    protected override void OnCollisionEnter(Collision other) 
    {
        GameObject impactExplosion;

        // Get the specified type from object pool and place it at the collision position
        impactExplosion = ObjectPool.GetPooledObject(PooledObjectType.MissileExplosion);
        impactExplosion.transform.position = transform.position;
        impactExplosion.SetActive(true);

        Destroy(gameObject);
    }
}
