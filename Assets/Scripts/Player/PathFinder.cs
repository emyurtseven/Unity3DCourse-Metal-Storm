using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Manages enemy movement in preset paths.
/// </summary>
public class PathFinder : MonoBehaviour
{
    // Parent container object that holds the bezier curves
    [SerializeField] Transform playerPath;

    // List of curves to follow
    List<CubicBezierCurve> curveList = new List<CubicBezierCurve>();

    [SerializeField] float moveSpeedMultiplier;
    [SerializeField] float turnSpeedMultiplier;

    bool isMoving = false;      // Only move along the curve if true

    CubicBezierCurve currentCurve;

    float moveSpeed;
    float speedFactor = 1f;

    // These are for movement smoothing along the bezier curve
    float targetStepSize;
    float lastStepSize;
    float errorTolerance;

    float t = 0;

    Vector3 previousPos;

    public bool IsMoving { get => isMoving; set => isMoving = value; }
    public float MoveSpeedMultiplier { get => moveSpeedMultiplier; set => moveSpeedMultiplier = value; }

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

        // These numbers were found solely with trial and error. No mathematical basis exists.
        targetStepSize = moveSpeedMultiplier / 50f;
        errorTolerance = targetStepSize / 30;

        StartCoroutine(FollowPath());
    }

    /// <summary>
    /// Look at the tangent vector (derivative) at the current point along the curve.
    /// </summary>
    private void LookForward()
    {
        Vector3 tangent = currentCurve.BezierTangent(t);

        Vector3 flattenedVecForBase;

        // If the current curve has pitchLocked = true;
        // flatten the y component so that the player always faces forward 
        // towards the horizon and not up and down
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
        if (isMoving)
        {
            FollowCurrentCurve();
        }
    }

    private IEnumerator FollowPath()
    {
        int index = 0;
        while(true)
        {
            t = 0;
            currentCurve = curveList[index];
            float curveLength = currentCurve.BezierSingleLength(currentCurve.ControlPointPositions);

            if (currentCurve.MoveSpeedOverride != 0)
            {
                moveSpeed = (Time.fixedDeltaTime / curveLength) * currentCurve.MoveSpeedOverride;
            }
            else
            {
                moveSpeed = (Time.fixedDeltaTime / curveLength) * moveSpeedMultiplier;
            }
            

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
        // t starts as 0 and goes to 1

        previousPos = transform.position;
        transform.position = currentCurve.BezierCubic(t);

        if (currentCurve.SpeedModulated)
        {
            ModulateSpeed();
        }

        LookForward();

        // Debug.DrawLine(transform.position, transform.position + tangent, Color.magenta);

        t += (moveSpeed * speedFactor);
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
    void ModulateSpeed()
    {
        lastStepSize = Vector3.Magnitude(transform.position - previousPos);

        // Debug.Log(lastStepSize);

        if (lastStepSize < targetStepSize - errorTolerance)
        {
            speedFactor *= 1.01f;
        }
        else if (lastStepSize > targetStepSize + errorTolerance)
        {
            speedFactor *= 0.99f;
        }
        else
        {
            speedFactor *= 1;
        }
    }
}
