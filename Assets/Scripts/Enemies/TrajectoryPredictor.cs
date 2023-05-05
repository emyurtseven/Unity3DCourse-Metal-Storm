using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Put this on the shooting game object and get a reference in the shooter script.
/// Use public PredictInterceptionPos() function to get a Vector3 position to shoot a projectile at.
/// </summary>
public class TrajectoryPredictor : MonoBehaviour
{
    // Speed of the launched projectile.
    float projectileSpeed;

    [Range(0.001f,1)]
    [SerializeField] float sensitivity = 0.01f;

    // Approximate offset distance to targets center towards which the projectile is fired.
    // Negative means in front of the target, positive means behind.
    [Tooltip("Approximate offset distance to targets center towards which the projectile is fired.\n Negative means in front of the target, positive means behind.")]
    [SerializeField] float leadingDistance = 0f;

    // Approximate max range in which a trajectory collision prediction will be made.
    float maxRange;

    bool initialized = false;

    bool targetUsesRigidbody;

    // Current world position of the shooting object
    float h;    
    float k;
    float l;


    Vector3 targetAcceleration;
    Vector3 lastVelocity;

    Rigidbody targetRigitbody;
    VelocityReader targetVelocityReader;

    GameObject targetObject;

    public float ProjectileSpeed { get => projectileSpeed; set => projectileSpeed = value; }
    public float MaxRange { get => maxRange; set => maxRange = value; }

    public void Initialize(GameObject target, float projectileSpeed)
    {
        lastVelocity = Vector3.zero;
        targetRigitbody = target.GetComponent<Rigidbody>();
        targetUsesRigidbody = (targetRigitbody != null && !targetRigitbody.isKinematic);
        targetVelocityReader = target.GetComponent<VelocityReader>();

        this.projectileSpeed = projectileSpeed;
        initialized = true;
    }

    /// <summary>
    /// *** DEBUGGING ONLY ***
    /// </summary>
    // private void Update() 
    // {
    //     if (Input.GetKeyDown(KeyCode.Space))
    //     {
    //         ShootDebugProjectile();
    //     }
    // }

    void FixedUpdate()
    {
        if (!initialized)
        {
            return;
        }

        if (targetUsesRigidbody)
        {
            targetAcceleration = (targetRigitbody.velocity - lastVelocity) / Time.fixedDeltaTime;
            lastVelocity = targetRigitbody.velocity;
        }
        else
        {
            targetAcceleration = targetVelocityReader.Acceleration;
        }
    }

    /// <summary>
    /// Calculates the position at which the projectile and the target will intersect.
    /// Returns world coordinates of the intersection. Returns null if none can be found.
    /// </summary>
    public Vector3 PredictInterceptionPos(GameObject target, float projectileSpeed)
    {
        if (!initialized)
        {
            Initialize(target, projectileSpeed);
        }

        Vector3 predictedInterceptionPos = Vector3.zero;
        Vector3 targetVelocity;

        // Get current position of this predicting object
        h = transform.position.x;
        k = transform.position.y;
        l = transform.position.z;

        // Initial position of the target at the time of prediction
        Vector3 targetInitialPos = target.transform.position;

        Vector3 targetNextPos;

        // Get target velocity
        if (targetUsesRigidbody)
        {
            targetVelocity = target.GetComponent<Rigidbody>().velocity;
        }
        else
        {
            targetVelocity = targetVelocityReader.AverageVelocity;
        }

        // At what intervals will the equation be checked?
        // Smaller steps of time means more sensitive and accurate prediction.
        float time = sensitivity;

        // For how many seconds into "future" we are going to check for a potential collision
        while(time < (maxRange / projectileSpeed))
        {
            targetNextPos = targetInitialPos + (targetVelocity * time) + (targetAcceleration * Mathf.Pow(time, 2)) / 2;

            float nextRadius = projectileSpeed * time;

            if (SphereEquation(targetNextPos, nextRadius))
            {
                predictedInterceptionPos = targetNextPos;
                break;
            }

            time += sensitivity;
        }

        return predictedInterceptionPos;
    }


    /// <summary>
    /// This function uses the equation of a sphere with radius r and world pos (h, k, l):
    /// 
    ///     (x - h)^2 + (y - k)^2 + (z - l)^2 = r^2
    /// 
    /// Where x, y, z represent points on the surface. 
    /// </summary>
    /// <param name="targetPos"> Current position of the target </param>
    /// <param name="r"> Radius of the sphere </param>
    /// <returns></returns>
    private bool SphereEquation(Vector3 targetPos, float r)
    {
        float x = targetPos.x;
        float y = targetPos.y;
        float z = targetPos.z;

        float firstValue = Mathf.Pow(x - h, 2);
        float secondValue = Mathf.Pow(y - k, 2);
        float thirdValue = Mathf.Pow(z - l, 2);

        float righthandSide = Mathf.Pow(r, 2);
        float lefthandSide = firstValue + secondValue + thirdValue;

        float acceptableValueRange = 100 * Mathf.Sign(leadingDistance) * Mathf.Pow(leadingDistance, 2);

        return ((lefthandSide - righthandSide) <= acceptableValueRange);
    }

    /// <summary>
    /// *** DEBUGGING ONLY ***
    /// </summary>
    public void ShootDebugProjectile()
    {
        GameObject projectile = GameObject.CreatePrimitive(PrimitiveType.Cube);
        projectile.transform.position = transform.position;

        Vector3 targetPos = PredictInterceptionPos(this.targetObject, this.projectileSpeed);
        Vector3 fromEnemyToPlayer = targetPos - transform.position;

        // Normalize it to length 1
        fromEnemyToPlayer.Normalize();

        // Set the speed to whatever you want:
        Vector3 velocity = fromEnemyToPlayer * projectileSpeed;

        Rigidbody rb = projectile.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.velocity = velocity;
    }
}
