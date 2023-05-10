using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Manages enemy movement in preset paths.
/// </summary>
public class PathFinder : MonoBehaviour
{
    // Parent container object that holds the bezier curves
    [SerializeField] Transform path;

    // List of curves to follow
    List<CubicBezierCurve> curveList = new List<CubicBezierCurve>();

    [SerializeField] float moveSpeedMultiplier = 15f;
    [SerializeField] float turnSpeedMultiplier = 15f;

    [SerializeField] bool isMoving = false;      // Only move along the curve if true

    bool isKinematic;

    CubicBezierCurve currentCurve;

    Rigidbody myRigidbody;

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

    protected virtual void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        if (myRigidbody == null)
        {
            isKinematic = true;
        }
        else
        {
            isKinematic = myRigidbody.isKinematic;

        }

        foreach (Transform curveTransform in path)
        {
            CubicBezierCurve curve = curveTransform.GetComponent<CubicBezierCurve>();
            curve.InitializeControlPoints();
            curveList.Add(curve);
        }

        // These numbers were found solely with trial and error. No mathematical basis exists.
        targetStepSize = moveSpeedMultiplier / 50f;
        errorTolerance = targetStepSize / 30;
    }

    private void FixedUpdate()
    {
        if (!isMoving)
        {
            return;
        }

        if (isKinematic)
        {
            FollowCurrentCurveKinematic();
        }
        else
        {
            FollowCurrentCurveDynamic();
        }
    }

    public IEnumerator FollowPath()
    {
        int index = 0;
        isMoving = true;
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
                isMoving = false;
                moveSpeed = 0;
                yield break;
            }
        }
    }

    /// <summary>
    /// Coroutine for following the curves
    /// </summary>
    private void FollowCurrentCurveKinematic()
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

        t += (moveSpeed * speedFactor);
    }

    /// <summary>
    /// Coroutine for following the curves
    /// </summary>
    private void FollowCurrentCurveDynamic()
    {
        myRigidbody.velocity = (currentCurve.BezierCubic(t) - transform.position);

        // if (currentCurve.SpeedModulated)
        // {
        //     ModulateSpeed();
        // }

        LookForward();

        t += (moveSpeed * speedFactor);
    }


    /// <summary>
    /// Look at the tangent vector (derivative) at the current point along the curve.
    /// </summary>
    private void LookForward()
    {
        Vector3 tangent = currentCurve.BezierTangent(t);
        Vector3 flattenedVecForBase;

        // If the current curve has pitchLocked = true;
        // flatten the y component so that the object always faces forward 
        // towards the horizon and isn't pitched up or down
        if (currentCurve.PitchLocked)
        {
            flattenedVecForBase = Vector3.ProjectOnPlane(tangent, transform.up);
        }
        else
        {
            flattenedVecForBase = tangent;
        }

        // *** DEBUG ONLY ***  draw a line that represents the direction object is facing
        // DrawUtilities.DrawArrowForDebug(transform.position, flattenedVecForBase, Color.magenta, 5f, 20f, 1f);

        transform.rotation = Quaternion.RotateTowards(
            Quaternion.LookRotation(transform.forward, transform.up),
            Quaternion.LookRotation(flattenedVecForBase, transform.up),
            turnSpeedMultiplier * Time.fixedDeltaTime);
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
