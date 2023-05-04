using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Manages enemy movement in preset paths
/// </summary>
public class PathFinder : MonoBehaviour
{
    List<Transform> waypoints;

    [SerializeField] Transform playerPath;

    // List of curves to follow
    List<CubicBezierCurve> curveList = new List<CubicBezierCurve>();

    [SerializeField] float moveSpeedMultiplier;
    [SerializeField] float turnSpeedMultiplier;

    CubicBezierCurve currentCurve;

    float moveSpeed;
    float speedFactor;

    float t = 0;

    // These are for movement smoothing along the bezier curve
    // float targetStepSize;
    // float lastStepSize;

    Vector3 previousPos;


    void Start()
    {
        foreach (Transform curveTransform in playerPath)
        {
            if (curveTransform.gameObject.activeInHierarchy)
            {
                CubicBezierCurve curve = curveTransform.GetComponent<CubicBezierCurve>();
                curve.InitializeControlPoints();
                curveList.Add(curve);
            }
        }

        StartCoroutine(FollowPath());
    }

    private void LookForward()
    {
        Vector3 tangent = currentCurve.BezierTangent(t);
        Vector3 flattenedVecForBase;
        
        if (currentCurve.PitchLocked)
        {
            flattenedVecForBase = Vector3.ProjectOnPlane(tangent, transform.up);
        }
        else
        {
            flattenedVecForBase = tangent;
        }

        transform.rotation = Quaternion.RotateTowards(
            Quaternion.LookRotation(transform.forward, transform.up),
            Quaternion.LookRotation(flattenedVecForBase, transform.up),
            turnSpeedMultiplier * Time.fixedDeltaTime);
    }

    private void FixedUpdate() 
    {
        FollowCurrentCurve();
    }

    private IEnumerator FollowPath()
    {
        int index = 0;
        while(true)
        {
            t = 0;
            currentCurve = curveList[index];
            float curveLength = currentCurve.BezierSingleLength(currentCurve.ControlPointPositions);
            moveSpeed = (Time.fixedDeltaTime / curveLength) * moveSpeedMultiplier;

            yield return new WaitUntil(() => CheckCurrentCurveCompleted(t));

            index++;

            if (index == curveList.Count)
            {
                moveSpeed = 0;
                yield break;
            }
        }
    }

    /// <summary>
    /// Coroutine for following the curves
    /// </summary>
    void FollowCurrentCurve()
    {
        // Take steps every frame along the bezier curve in relation to the parameter t, 
        // while t is smaller than the boundary condition.
        // t starts as 0 and goes to 1, or vice versa if reversed

        previousPos = transform.position;
        transform.position = currentCurve.BezierCubic(t);
        // ModulateSpeed();

        LookForward();

        // Debug.DrawLine(transform.position, transform.position + tangent, Color.magenta);

        t += moveSpeed;
    }

    /// <summary>
    /// Check if current curve is completed, ie t = boundary
    /// </summary>
    /// <param name="t"></param>
    /// <returns>Returns a bool indicating if completed</returns>
    private bool CheckCurrentCurveCompleted(float t)
    {
        return (t >= 1);
    }

    /// <summary>
    /// Smmothe the movement speed along the bezier curve
    /// </summary>
    // void ModulateSpeed()
    // {
    //     lastStepSize = Vector3.Magnitude(transform.position - previousPos);
    //     if (lastStepSize < targetStepSize)
    //     {
    //         speedFactor *= 1.1f;
    //     }
    //     else
    //     {
    //         speedFactor *= 0.9f;
    //     }
    // }
}
