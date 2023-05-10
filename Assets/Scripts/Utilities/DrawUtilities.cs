using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class DrawUtilities
{
#if UNITY_EDITOR

    public static void DrawArrowForGizmo(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f, float arrowPosition = 1f)
    {
        DrawArrowForGizmo(pos, direction, Gizmos.color, arrowHeadLength, arrowHeadAngle, arrowPosition);
    }

    public static void DrawArrowForGizmo(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f, float arrowPosition = 1f)
    {
        Gizmos.color = color;
        Gizmos.DrawRay(pos, direction);
        DrawArrowEnd(true, pos, direction, color, arrowHeadLength, arrowHeadAngle, arrowPosition);
    }

    public static void DrawArrowForDebug(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f, float arrowPosition = 1f)
    {
        DrawArrowForDebug(pos, direction, Color.white, arrowHeadLength, arrowHeadAngle, arrowPosition);
    }

    public static void DrawArrowForDebug(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f, float arrowPosition = 1f)
    {
        Debug.DrawRay(pos, direction, color);
        DrawArrowEnd(false, pos, direction, color, arrowHeadLength, arrowHeadAngle, arrowPosition);
    }
    private static void DrawArrowEnd(bool gizmos, Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f, float arrowPosition = 1f)
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
    
    public static void DrawCompleteArrow(Vector3 a, Vector3 b, float arrowheadAngle, float arrowheadDistance, float arrowheadLength)
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

    public static void DrawSphereWithLabel(Vector3 position, Vector3 targetPosition, float radius, Color color, float textSize=10, string label="No Label")
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(position, radius);

        Vector3 textPosition = position -
                        (Vector3.Normalize(position - targetPosition) * radius);

        DrawUtilities.DrawString(label, textPosition, color, Vector2.zero, textSize);
    }

    public static void DrawString(string text, Vector3 worldPosition, Color textColor, Vector2 anchor, float textSize = 15f)
    {
        var view = UnityEditor.SceneView.currentDrawingSceneView;
        if (!view)
            return;
        Vector3 screenPosition = view.camera.WorldToScreenPoint(worldPosition);
        if (screenPosition.y < 0 || screenPosition.y > view.camera.pixelHeight || screenPosition.x < 0 || screenPosition.x > view.camera.pixelWidth || screenPosition.z < 0)
            return;
        // var pixelRatio = UnityEditor.HandleUtility.GUIPointToScreenPixelCoordinate(Vector2.right).x - UnityEditor.HandleUtility.GUIPointToScreenPixelCoordinate(Vector2.zero).x;
        var pixelRatio = EditorGUIUtility.pixelsPerPoint;
        UnityEditor.Handles.BeginGUI();
        var style = new GUIStyle(GUI.skin.label)
        {
            fontSize = (int)textSize,
            normal = new GUIStyleState() { textColor = textColor }
        };
        Vector2 size = style.CalcSize(new GUIContent(text)) * pixelRatio;
        var alignedPosition =
            ((Vector2)screenPosition +
            size * ((anchor + Vector2.left + Vector2.up) / 2f)) * (Vector2.right + Vector2.down) +
            Vector2.up * view.camera.pixelHeight;
        GUI.Label(new Rect(alignedPosition / pixelRatio, size / pixelRatio), text, style);
        UnityEditor.Handles.EndGUI();

    }

#endif
}
