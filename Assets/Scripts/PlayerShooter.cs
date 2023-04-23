using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerShooter : MonoBehaviour
{
    [SerializeField] float cannonDamage;
    [SerializeField] float rocketDamage;
    bool isFiringGun;

    [SerializeField] ParticleSystem gunParticles;

    public float CannonDamage { get => cannonDamage; }
    public float RocketDamage { get => rocketDamage; }

    void Update()
    {
        FireGun();
    }

    private void FireGun()
    {
        if (gunParticles.isEmitting)
        {
            if (!isFiringGun)
            {
                gunParticles.Stop();
            }
        }
        else
        {
            if (isFiringGun)
            {
                gunParticles.Play();
            }
        }
    }

    private void OnFireGun(InputValue value)
    {
        isFiringGun = value.isPressed;
    }
}
