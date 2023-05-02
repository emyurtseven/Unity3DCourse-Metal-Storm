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
    [SerializeField] float projectileSpeed = 5f;

    [Range(0.001f,1)]
    [SerializeField] float sensitivity = 0.01f;

    // Approximate distance to targets center towards which the projectile is fired.
    // Negative means in front of the target, positive means behind.
    [Tooltip("Approximate distance to targets center towards which the projectile is fired.\n Negative means in front of the target, positive means behind.")]
    [SerializeField] float leadingDistance = 0f;

    // Approximate max range in which a trajectory collision prediction will be made.
    [SerializeField] float maxRange;

    [SerializeField] GameObject target;

    // Current world position of the shooting object
    float h;    
    float k;
    float l;

    Vector3 targetAcceleration;
    Vector3 lastVelocity;

    Rigidbody rb;

    private void Start() 
    {
        lastVelocity = Vector3.zero;
        rb = target.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        targetAcceleration = (rb.velocity - lastVelocity) / Time.fixedDeltaTime;
        lastVelocity = rb.velocity;
    }

    /// <summary>
    /// Calculates the position at which the projectile and the target will intersect.
    /// Returns world coordinates of the intersection. Returns null if none can be found.
    /// </summary>
    public Vector3? PredictInterceptionPos(GameObject target)
    {
        Vector3? predictedInterceptionPos = null;

        // Get current position of this predicting object
        h = transform.position.x;
        k = transform.position.y;
        l = transform.position.z;

        // Initial position of the target at the time of prediction
        Vector3 targetInitialPos = target.transform.position;

        Vector3 targetNextPos;
        Vector3 targetVelocity = target.GetComponent<Rigidbody>().velocity;

        // At what intervals of time will the equation be checked.
        // Smaller means more sensitive and accurate prediction.
        float t = sensitivity;

        // 
        while(t < (maxRange / projectileSpeed))
        {
            targetNextPos = targetInitialPos + (targetVelocity * t) + (targetAcceleration * Mathf.Pow(t, 2)) / 2;

            float nextRadius = projectileSpeed * t;

            if (SphereEquation(targetNextPos, nextRadius))
            {
                predictedInterceptionPos = targetNextPos;
                break;
            }

            t += sensitivity;
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
}
