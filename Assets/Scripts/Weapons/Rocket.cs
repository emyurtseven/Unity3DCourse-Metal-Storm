using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : Weapon
{
    float rocketSpeed = 3f;
    float armingDelay = 1f;
    Rigidbody myRigidbody;

    bool isFired;

    public float RocketSpeed { get => rocketSpeed; set => rocketSpeed = value; }

    protected override void Start() 
    {
        this.Type = WeaponType.Rocket;
        myRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isFired)
        {
            myRigidbody.AddRelativeForce(Vector3.forward * RocketSpeed, ForceMode.Force);
        }
    }

    public void LaunchRocket()
    {
        gameObject.transform.parent = null;
        GetComponent<AudioSource>().Play();
        GetComponent<ParticleSystem>().Play();
        isFired = true;

        Invoke("ArmRocket", armingDelay);
    }

    private void ArmRocket()
    {
        GetComponent<Collider>().enabled = true;
    }

    protected override void OnCollisionEnter(Collision other) 
    {
        GameObject impactExplosion;

        // Get the specified type from object pool and place it at the collision position
        impactExplosion = ObjectPool.GetPooledObject(PooledObjectType.Explosion);
        impactExplosion.transform.position = transform.position;
        impactExplosion.SetActive(true);

        Destroy(gameObject);
    }
}
