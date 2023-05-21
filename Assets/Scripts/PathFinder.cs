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

    Transform targetToLookAt;
    CubicBezierCurve currentCurve;
    Rigidbody myRigidbody;

    bool isKinematic;

    float moveSpeed;
    float speedFactor = 1f;

    // These are for movement smoothing along the bezier curve
    float targetStepSize;
    float lastStepSize;
    float errorTolerance;

    float t = 0;

    Vector3 previousPos;
    PlayerDestroyedEvent playerDestroyed = new PlayerDestroyedEvent();

    public bool IsMoving { get => isMoving; set => isMoving = value; }
    public float MoveSpeedMultiplier { get => moveSpeedMultiplier; set => moveSpeedMultiplier = value; }

    private void Awake() 
    {
        EventManager.AddNoArgumentListener(DisableMovement, EventType.PlayerDestroyed);
    }

    protected virtual void Start()
    {
        // Destroy path finder script if no path is found
        if (path == null)
        {
            Debug.LogWarning($"Path not set on game object: {gameObject.name}");
            Destroy(this);
            return;
        }

        Initialize();
    }

    private void FixedUpdate()
    {
        if (!isMoving)
        {
            return;
        }

        FollowPath();
    }

    /// <summary>
    /// Initialize components and curves.
    /// </summary>
    private void Initialize()
    {
        if (TryGetComponent<Rigidbody>(out myRigidbody))
        {
            isKinematic = myRigidbody.isKinematic;
        }
        else
        {
            isKinematic = true;
        }

        foreach (Transform curveTransform in path)
        {
            if (curveTransform.gameObject.activeSelf)
            {
                CubicBezierCurve curve = curveTransform.GetComponent<CubicBezierCurve>();
                curve.InitializeControlPoints();
                curveList.Add(curve);
            }
        }

        // These numbers were found solely with trial and error. No known mathematical basis :)
        targetStepSize = moveSpeedMultiplier / 50f;
        errorTolerance = targetStepSize / 30;
    }


    /// <summary>
    /// Traverse the path on current curve and rotate to face a direction
    /// </summary>
    private void FollowPath()
    {
        if (isKinematic)
        {
            FollowCurrentCurveKinematic();
        }
        else
        {
            FollowCurrentCurveDynamic();
        }

        if (targetToLookAt != null)
        {
            LookAt(targetToLookAt.position);
        }
        else
        {
            LookAt();
        }

        t += (moveSpeed * speedFactor);
    }

    /// <summary>
    /// Coroutine that iterates curves in curveList. 
    /// Sets t = 0 before every curve, t is then updated in FixedUpdate()
    /// </summary>
    public IEnumerator IterateOverCurves()
    {
        int index = 0;
        isMoving = true;

        while(true)
        {
            t = 0;
            currentCurve = curveList[index];
            float curveLength = currentCurve.BezierSingleLength(currentCurve.ControlPointPositions);

            // Adjust speed if the curve has an overriding value. 1 means no override
            moveSpeed = (Time.fixedDeltaTime / curveLength) * moveSpeedMultiplier * currentCurve.MoveSpeedOverride;

            // Wait until t = 1, which means current curve is completed
            yield return new WaitUntil(() => CheckCurrentCurveCompleted(t));

            index++;

            // If curvelist is exhausted, stop and break
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
    }

    /// <summary>
    /// Coroutine for following the curves
    /// </summary>
    private void FollowCurrentCurveDynamic()
    {
        Vector3 velocityDirection = (currentCurve.BezierCubic(t) - transform.position);
        myRigidbody.velocity = Vector3.ProjectOnPlane(velocityDirection, transform.up);
    }


    /// <summary>
    /// Look at the target object that is set in the current curve, or if there's no target
    /// look at the tangent vector (forward) of the bezier curve at given point.
    /// </summary>
    private void LookAt(Vector3? targetPosition=null)
    {
        Vector3 direction;
        Vector3 flattenedVecForBase;

        if (targetPosition == null)
        {
            direction = currentCurve.BezierTangent(t);
        }
        else
        {
            direction = (Vector3)targetPosition - transform.position;
        }
        

        // If the current curve has pitchLocked = true;
        // flatten the y component so that the object always faces forward 
        // towards the horizon and isn't pitched up or down
        if (currentCurve.PitchLocked)
        {
            flattenedVecForBase = Vector3.ProjectOnPlane(direction, transform.up);
        }
        else
        {
            flattenedVecForBase = direction;
        }

        // *** DEBUG ONLY ***  draw a line that represents the direction object is facing
        DrawUtilities.DrawArrowForDebug(transform.position, flattenedVecForBase, Color.magenta, 5f, 20f, 1f);

        transform.rotation = Quaternion.RotateTowards(
            Quaternion.LookRotation(transform.forward, transform.up),
            Quaternion.LookRotation(flattenedVecForBase, transform.up),
            turnSpeedMultiplier * Time.fixedDeltaTime);
    }

    /// <summary>
    /// Tag = "LookTarget" objects have an entry and an exit trigger. 
    /// Sets the target to face towards on first trigger, sets it to null on second trigger.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.parent.tag == "LookTarget")
        {
            if (targetToLookAt == null)
            {
                targetToLookAt = other.transform.parent;
            }
            else
            {
                targetToLookAt = null;
            }
        }
    }

    /// <summary>
    /// Check if current curve is completed, ie t = boundary
    /// </summary>
    /// <returns>Returns a bool indicating if completed</returns>
    private bool CheckCurrentCurveCompleted(float t)
    {
        return (t >= 1);
    }

    /// <summary>
    /// Smoothe the movement speed along the bezier curve
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

    protected void DisableMovement()
    {
        isMoving = false;
    }

    /// <summary>
    /// Draws every curve in objects path upon selecting, for visual aid on inspector.
    /// </summary>
    protected virtual void OnDrawGizmosSelected()
    {
        if (path == null)
        {
            return;
        }
        foreach (Transform curveTransform in path)
        {
            CubicBezierCurve curve = curveTransform.GetComponent<CubicBezierCurve>();
            curve.InitializeControlPoints();

            Gizmos.color = curve.gizmosDotColor;

            for (float t = 0; t <= 1; t += curve.gizmosDotDistance)
            {
                Gizmos.DrawSphere(curve.BezierCubic(t), curve.gizmosDotRadius);
            }

            Gizmos.color = curve.gizmosBezierPointColor;
            for (int i = 0; i < 4; i++)
            {
                Gizmos.DrawSphere(curve.ControlPoints[i].position, curve.gizmosBezierPointRadius);
            }

            Gizmos.color = curve.gizmosEndTangentColor;
            Gizmos.DrawLine(curve.ControlPoints[3].position, curve.ControlPoints[3].position + (curve.BezierTangent(1) / 4));
            Gizmos.color = curve.gizmosStartTangentColor;
            Gizmos.DrawLine(curve.ControlPoints[0].position, curve.ControlPoints[0].position + (curve.BezierTangent(0) / 3));

            if (curve.drawLines)
            {
                Gizmos.DrawLine(new Vector3(curve.ControlPoints[0].position.x, curve.ControlPoints[0].position.y, curve.ControlPoints[0].position.z),
                                new Vector3(curve.ControlPoints[1].position.x, curve.ControlPoints[1].position.y, curve.ControlPoints[1].position.z));

                Gizmos.DrawLine(new Vector3(curve.ControlPoints[2].position.x, curve.ControlPoints[2].position.y, curve.ControlPoints[2].position.z),
                                new Vector3(curve.ControlPoints[3].position.x, curve.ControlPoints[3].position.y, curve.ControlPoints[3].position.z));
            }
        }
    }
}
