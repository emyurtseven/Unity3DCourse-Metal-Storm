using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerShooter : MonoBehaviour
{
    [SerializeField] float cannonDamage;
    [SerializeField] float rocketDamage;
    bool isFiringGun;

    [SerializeField] GameObject cannon;
    [SerializeField] GameObject muzzleFlash;

    ParticleSystem cannonParticles;

    public float CannonDamage { get => cannonDamage; }
    public float RocketDamage { get => rocketDamage; }

    private void Start() 
    {
        cannonParticles = cannon.GetComponent<ParticleSystem>();
    }

    void Update()
    {
        RotateCannon();
        FireCannon();
    }

    private void RotateCannon()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            cannon.transform.LookAt(hit.point);
        }
    }

    private void FireCannon()
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
