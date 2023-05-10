using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DrawArrow
{
    public static void ForGizmo(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f, float arrowPosition = 0.5f)
    {
        ForGizmo(pos, direction, Gizmos.color, arrowHeadLength, arrowHeadAngle, arrowPosition);
    }

    public static void ForGizmo(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f, float arrowPosition = 0.5f)
    {
        Gizmos.color = color;
        Gizmos.DrawRay(pos, direction);
        DrawArrowEnd(true, pos, direction, color, arrowHeadLength, arrowHeadAngle, arrowPosition);
    }

    public static void ForDebug(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f, float arrowPosition = 0.5f)
    {
        ForDebug(pos, direction, Color.white, arrowHeadLength, arrowHeadAngle, arrowPosition);
    }

    public static void ForDebug(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f, float arrowPosition = 0.5f)
    {
        Debug.DrawRay(pos, direction, color);
        DrawArrowEnd(false, pos, direction, color, arrowHeadLength, arrowHeadAngle, arrowPosition);
    }
    private static void DrawArrowEnd(bool gizmos, Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f, float arrowPosition = 0.5f)
    {
        Vector3 right = (Quaternion.LookRotation(direction) * Quaternion.Euler(arrowHeadAngle, 0, 0) * Vector3.back) * arrowHeadLength;
        Vector3 left = (Quaternion.LookRotation(direction) * Quaternion.Euler(-arrowHeadAngle, 0, 0) * Vector3.back) * arrowHeadLength;
        Vector3 up = (Quaternion.LookRotation(direction) * Quaternion.Euler(0, arrowHeadAngle, 0) * Vector3.back) * arrowHeadLength;
        Vector3 down = (Quaternion.LookRotation(direction) * Quaternion.Euler(0, -arrowHeadAngle, 0) * Vector3.back) * arrowHeadLength;

        Vector3 arrowTip = pos + (direction * arrowPosition);

        if (gizmos)
        {
            Gizmos.color = color;
            Gizmos.DrawRay(arrowTip, right);
            Gizmos.DrawRay(arrowTip, left);
            Gizmos.DrawRay(arrowTip, up);
            Gizmos.DrawRay(arrowTip, down);
        }
        else
        {
            Debug.DrawRay(arrowTip, right, color);
            Debug.DrawRay(arrowTip, left, color);
            Debug.DrawRay(arrowTip, up, color);
            Debug.DrawRay(arrowTip, down, color);
        }
    }
    
    public static void DrawArrowEnd(Vector3 a, Vector3 b, float arrowheadAngle, float arrowheadDistance, float arrowheadLength)
    {
        // Get the Direction of the Vector
        Vector3 dir = b - a;

        // Get the Position of the Arrowhead along the length of the line.
        Vector3 arrowPos = a + (dir * arrowheadDistance);

        // Get the Arrowhead Lines using the direction from earlier multiplied by a vector representing half of the full angle of the arrowhead (y)
        // and -1 for going backwards instead of forwards (z), which is then multiplied by the desired length of the arrowhead lines coming from the point.

        Vector3 up = Quaternion.LookRotation(dir) * new Vector3(0f, Mathf.Sin(arrowheadAngle * Mathf.Deg2Rad), -1f) * arrowheadLength;
        Vector3 down = Quaternion.LookRotation(dir) * new Vector3(0f, -Mathf.Sin(arrowheadAngle * Mathf.Deg2Rad), -1f) * arrowheadLength;
        Vector3 left = Quaternion.LookRotation(dir) * new Vector3(Mathf.Sin(arrowheadAngle * Mathf.Deg2Rad), 0f, -1f) * arrowheadLength;
        Vector3 right = Quaternion.LookRotation(dir) * new Vector3(-Mathf.Sin(arrowheadAngle * Mathf.Deg2Rad), 0f, -1f) * arrowheadLength;

        // Get the End Locations of all points for connecting arrowhead lines.
        Vector3 upPos = arrowPos + up;
        Vector3 downPos = arrowPos + down;
        Vector3 leftPos = arrowPos + left;
        Vector3 rightPos = arrowPos + right;

        // Draw the line from A to B
        Gizmos.DrawLine(a, b);

        // Draw the rays representing the arrowhead.
        Gizmos.DrawRay(arrowPos, up);
        Gizmos.DrawRay(arrowPos, down);
        Gizmos.DrawRay(arrowPos, left);
        Gizmos.DrawRay(arrowPos, right);

        // Draw Connections between rays representing the arrowhead
        Gizmos.DrawLine(upPos, leftPos);
        Gizmos.DrawLine(leftPos, downPos);
        Gizmos.DrawLine(downPos, rightPos);
        Gizmos.DrawLine(rightPos, upPos);

    }
}
