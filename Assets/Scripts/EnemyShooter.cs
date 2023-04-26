using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : Shooter
{
    [SerializeField] float firingDuration;
    [SerializeField] float idleDuration;

    Timer timer;

    protected override void Start() 
    {
        base.Start();

        timer = gameObject.AddComponent<Timer>();
        timer.Duration = idleDuration;
        timer.Run();
        isFiringGun = false;
    }

    protected new void Update() 
    {
        AlternateFiring();       // Enemies only
        FireCannon();
        EnemyCannonAudio();
    }

    private void AlternateFiring()
    {
        if (timer.Finished && isFiringGun)
        {
            timer.Duration = idleDuration;
            timer.Run();
            isFiringGun = false;
        }
        else if (timer.Finished && !isFiringGun)
        {
            timer.Duration = firingDuration;
            timer.Run();
            isFiringGun = true;
        }
    }

    private void EnemyCannonAudio()
    {
        if (isFiringGun && !cannonAudioSource.isPlaying)
        {
            cannonAudioSource.Play();
        }
        else if (!isFiringGun && cannonAudioSource.isPlaying)
        {
            cannonAudioSource.Stop();
        }
    }
}
