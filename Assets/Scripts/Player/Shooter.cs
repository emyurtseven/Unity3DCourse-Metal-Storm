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

    [SerializeField] protected AudioSource cannonAudioSource;

    protected bool isFiringGun;
    bool gunWindingDown = true;

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
        PlayerCannonAudio();
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

    private void PlayerCannonAudio()
    {
        if (isFiringGun && gunWindingDown)
        {
            cannonAudioSource.Stop();
            cannonAudioSource.Play();
            gunWindingDown = false;
        }
        else if (cannonAudioSource.isPlaying && !isFiringGun && !gunWindingDown)
        {
            cannonAudioSource.time = 3.9f;
            gunWindingDown = true;
        }
        else if (isFiringGun && cannonAudioSource.time >= 3 && !gunWindingDown)
        {
            cannonAudioSource.time = 1f;
        }
    }

    private void OnFireGun(InputValue value)
    {
        isFiringGun = value.isPressed;
    }
}
