using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniRocket : Weapon
{
    Rigidbody myRigidbody;
    AudioClip explosionSfx;

    float rocketSpeed;

    public float RocketSpeed { get => rocketSpeed; set => rocketSpeed = value; }

    protected override void Start()
    {
        int i = Random.Range(0, 3);
        string explosionName = $"rocket_explosion_outdoors ({i})";
        explosionSfx = (AudioClip)Resources.Load("Audio/" + explosionName);
        this.Type = WeaponType.MiniRocket;
        myRigidbody = GetComponent<Rigidbody>();

        Invoke("ArmRocket", 0.2f);
        base.Start();
    }

    protected override void OnCollisionEnter(Collision other)
    {
        GameObject impactExplosion;

        // Get the specified type from object pool and place it at the collision position
        impactExplosion = ObjectPool.GetPooledObject(PooledObjectType.Effects_Impacts_MiniRocket);
        impactExplosion.transform.position = transform.position;
        impactExplosion.GetComponent<AudioSource>().clip = explosionSfx;
        impactExplosion.SetActive(true);

        Destroy(gameObject);
    }


    private void ArmRocket()
    {
        GetComponent<Collider>().enabled = true;
    }
}
