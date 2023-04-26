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
}
