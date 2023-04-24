using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerShooter : MonoBehaviour
{
    [SerializeField] float cannonDamage;
    [SerializeField] float rocketDamage;
    bool isFiringGun;

    [SerializeField] ParticleSystem cannonParticles;
    [SerializeField] GameObject muzzleFlash;

    public float CannonDamage { get => cannonDamage; }
    public float RocketDamage { get => rocketDamage; }

    void Update()
    {
        FireGun();
    }

    private void FireGun()
    {
        if (cannonParticles.isEmitting)
        {
            if (!isFiringGun)
            {
                cannonParticles.Stop();
                muzzleFlash.SetActive(false);
            }
        }
        else
        {
            if (isFiringGun)
            {
                cannonParticles.Play();
                muzzleFlash.SetActive(true);
            }
        }
    }

    private void OnFireGun(InputValue value)
    {
        isFiringGun = value.isPressed;
    }
}
