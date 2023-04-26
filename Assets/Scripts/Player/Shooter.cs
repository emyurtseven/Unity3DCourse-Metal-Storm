using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Shooter : MonoBehaviour
{
    [SerializeField] float cannonDamagePerShot;
    [SerializeField] float missileDamagePerShot;

    [SerializeField] GameObject[] cannons;
    [SerializeField] GameObject[] muzzleFlashes;

    ParticleSystem[] cannonParticles;

    protected bool isFiringGun;

    protected virtual void Start() 
    {
        cannonParticles = new ParticleSystem[cannons.Length];
        
        for (int i = 0; i < cannons.Length; i++)
        {
            cannonParticles[i] = cannons[i].GetComponent<ParticleSystem>();
            cannons[i].GetComponent<Weapon>().DamagePerShot = cannonDamagePerShot;
        }
    }

    protected void Update()
    {
        RotatePlayerCannon();
        FireCannon();
    }


    private void RotatePlayerCannon()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            cannons[0].transform.LookAt(hit.point);
        }
    }

    protected void FireCannon()
    {
        for (int i = 0; i < cannonParticles.Length; i++)
        {
            if (cannonParticles[i].isEmitting)
            {
                if (!isFiringGun)
                {
                    cannonParticles[i].Stop();
                    muzzleFlashes[i].SetActive(false);
                }
            }
            else
            {
                if (isFiringGun)
                {
                    cannonParticles[i].Play();
                    muzzleFlashes[i].SetActive(true);
                }
            }
        }
    }

    private void OnFireGun(InputValue value)
    {
        isFiringGun = value.isPressed;
    }
}
