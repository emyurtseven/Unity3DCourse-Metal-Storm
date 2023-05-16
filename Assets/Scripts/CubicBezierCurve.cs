using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// Attached to a curve object, has 4 coordinates for calculating a cubic bezier curve.
/// Draw gizmos in scene view to visualize the curve. 
/// </summary>
public class CubicBezierCurve : MonoBehaviour
{
    [SerializeField] public bool drawLines;
    [SerializeField] public Color gizmosDotColor = Color.yellow;
    [SerializeField] public Color gizmosBezierPointColor = Color.red;
    [SerializeField] public Color gizmosEndTangentColor = Color.cyan;
    [SerializeField] public Color gizmosStartTangentColor = Color.blue;
    [SerializeField] public float gizmosDotRadius = 0.5f;
    [SerializeField] public float gizmosBezierPointRadius = 1f;
    [SerializeField] public float gizmosDotDistance = 0.05f;

    [SerializeField] bool pitchLocked = true;
    [SerializeField] bool speedModulated = true;
    
    [SerializeField] float moveSpeedOverride = 1f;

    Transform[] controlPoints = new Transform[4];
    Vector3[] controlPointPositions = new Vector3[4];


    public Transform[] Waypoints
    {
        get { return controlPoints; }
    }

    public Vector3[] ControlPointPositions 
    { 
        get 
        {
            int i = 0;
            foreach (Transform controlPoint in controlPoints)
            {
                controlPointPositions[i] = controlPoint.position;
                i++;
            }
            return controlPointPositions;
        }
    }

    public bool PitchLocked { get => pitchLocked; set => pitchLocked = value; }
    public bool SpeedModulated { get => speedModulated; set => speedModulated = value; }
    public float MoveSpeedOverride { get => moveSpeedOverride; set => moveSpeedOverride = value; }
    public Transform[] ControlPoints { get => controlPoints; set => controlPoints = value; }

    /// <summary>
    /// Find the child objects which are points in scene and store their coordinates
    /// </summary>
    public void InitializeControlPoints()
    {
        int index = 0;
        foreach (Transform child in transform)
        {
            controlPoints[index] = child;
            index++;
        }
    }

    /// <summary>
    /// Apply bezier transform to parameter t
    /// Formula: https://en.wikipedia.org/wiki/B%C3%A9zier_curve#Cubic_B%C3%A9zier_curves
    /// </summary>
    /// <param name="t"> Parameter t, 0->1 or vice versa </param>
    /// <returns> Returns 2D coordinates of resultant transformation </returns>
    public Vector3 BezierCubic(float t)
    {
        Vector3 component1 = Mathf.Pow((1 - t), 3) * controlPoints[0].position;
        Vector3 component2 = 3 * Mathf.Pow((1 - t), 2) * t * controlPoints[1].position;
        Vector3 component3 = 3 * (1 - t) * Mathf.Pow(t, 2) * controlPoints[2].position;
        Vector3 component4 = Mathf.Pow(t, 3) * controlPoints[3].position;

        Vector3 currentPos = component1 + component2 + component3 + component4;

        return currentPos;
    }

    public Vector3 BezierTangent(float t)
    {
        Vector3 component1 =  -3 * Mathf.Pow((1 - t), 2) * controlPoints[0].position;
        Vector3 component2 = 3 * Mathf.Pow((1 - t), 2) * controlPoints[1].position + 
                            (-6 * t * (1 - t) * controlPoints[1].position);
        Vector3 component3 = (-3 * Mathf.Pow(t, 2) * controlPoints[2].position) +
                            (6 * t * (1 - t) * controlPoints[2].position);
        Vector3 component4 = 3 * Mathf.Pow(t, 2) * controlPoints[3].position;

        Vector3 tangent = component1 + component2 + component3 + component4;

        return tangent;
    }


    public float BezierSingleLength(Vector3[] points)
    {
        var p0 = points[0] - points[1];
        var p1 = points[2] - points[1];
        var p2 = new Vector3();
        var p3 = points[3]-points[2];

        var l0 = p0.magnitude;
        var l1 = p1.magnitude;
        var l3 = p3.magnitude;
        if(l0 > 0) p0 /= l0;
        if(l1 > 0) p1 /= l1;
        if(l3 > 0) p3 /= l3;

        p2 = -p1;
        var a = Mathf.Abs(Vector3.Dot(p0,p1)) + Mathf.Abs(Vector3.Dot(p2,p3));
        if(a > 1.98f || l0 + l1 + l3 < (4 - a)*8) return l0+l1+l3;

        var bl = new Vector3[4];
        var br = new Vector3[4];

        bl[0] = points[0];
        bl[1] = (points[0]+points[1]) * 0.5f;

        var mid = (points[1]+points[2]) * 0.5f;

        bl[2] = (bl[1]+mid) * 0.5f;
        br[3] = points[3];
        br[2] = (points[2]+points[3]) * 0.5f;
        br[1] = (br[2]+mid) * 0.5f;
        br[0] = (br[1]+bl[2]) * 0.5f;
        bl[3] = br[0];

        return BezierSingleLength(bl) + BezierSingleLength(br);
    }

    /// <summary>
    /// Update drawn gizmos in scene view for visual help
    /// </summary>
    void OnDrawGizmos()
    {
        InitializeControlPoints();

        Gizmos.color = gizmosDotColor;

        for (float t = 0; t <= 1; t += gizmosDotDistance)
        {
            Gizmos.DrawSphere(BezierCubic(t), gizmosDotRadius);
        }

        Gizmos.color = gizmosBezierPointColor;
        for (int i = 0; i < 4; i++)
        {
            Gizmos.DrawSphere(controlPoints[i].position, gizmosBezierPointRadius);
        }

        Gizmos.color = gizmosEndTangentColor;
        Gizmos.DrawLine(controlPoints[3].position, controlPoints[3].position + (BezierTangent(1) / 4));
        Gizmos.color = gizmosStartTangentColor;
        Gizmos.DrawLine(controlPoints[0].position, controlPoints[0].position + (BezierTangent(0) / 3));

        if (drawLines)
        {
            Gizmos.DrawLine(new Vector3(controlPoints[0].position.x, controlPoints[0].position.y, controlPoints[0].position.z),
                            new Vector3(controlPoints[1].position.x, controlPoints[1].position.y, controlPoints[1].position.z));

            Gizmos.DrawLine(new Vector3(controlPoints[2].position.x, controlPoints[2].position.y, controlPoints[2].position.z),
                            new Vector3(controlPoints[3].position.x, controlPoints[3].position.y, controlPoints[3].position.z));
        }
    }
}
