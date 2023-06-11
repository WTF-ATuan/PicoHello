using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

class Debug : UnityEngine.Debug
{

    public static void DrawSphere(Vector3 pos, float Radius, Color32 color)
    {
        int segments = 32;
        float angle = 0f;
        Vector3 lastPoint = Vector3.zero;
        Vector3 thisPoint = Vector3.zero;

        for (int i = 0; i < segments + 1; i++)
        {
            thisPoint.x = Mathf.Sin(Mathf.Deg2Rad * angle) * Radius;
            thisPoint.y = Mathf.Cos(Mathf.Deg2Rad * angle) * Radius;

            Quaternion rot = Quaternion.LookRotation(Vector3.forward, Vector3.up);

            DrawLine(rot * lastPoint + pos, rot * thisPoint + pos, color);

            rot = Quaternion.LookRotation(Vector3.up, Vector3.forward);
            DrawLine(rot * lastPoint + pos, rot * thisPoint + pos, color);

            rot = Quaternion.LookRotation(Vector3.right, Vector3.forward);
            DrawLine(rot * lastPoint + pos, rot * thisPoint + pos, color);


            lastPoint = thisPoint;
            angle += 360f / segments;
        }

    }

    public static void DrawCircle(Vector3 pos, float Radius, Color32 color)
    {
        Vector3 forward = Vector3.up;
        Vector3 up = Vector3.forward;

        int segments = 32;
        float angle = 0f;
        Quaternion rot = Quaternion.LookRotation(forward, up);
        Vector3 lastPoint = Vector3.zero;
        Vector3 thisPoint = Vector3.zero;

        for (int i = 0; i < segments + 1; i++)
        {
            thisPoint.x = Mathf.Sin(Mathf.Deg2Rad * angle) * Radius;
            thisPoint.y = Mathf.Cos(Mathf.Deg2Rad * angle) * Radius;

            DrawLine(rot * lastPoint + pos, rot * thisPoint + pos, color);


            lastPoint = thisPoint;
            angle += 360f / segments;
        }
    }
    public static void DrawSphere_Local(Transform _T, float Radius, Color32 color)
    {
        Vector3 pos = _T.position;
        Vector3 Scale = _T.localScale * Radius;

        int segments = 32;
        float angle = 0f;
        Vector3 lastPoint = Vector3.zero;
        Vector3 thisPoint = Vector3.zero;

        for (int i = 0; i < segments + 1; i++)
        {
            thisPoint.x = Mathf.Sin(Mathf.Deg2Rad * angle) * Scale.x;
            thisPoint.y = Mathf.Cos(Mathf.Deg2Rad * angle) * Scale.y;


                Quaternion rot = Quaternion.LookRotation(Vector3.forward, Vector3.up);

                DrawLine(rot * lastPoint + pos, rot * thisPoint + pos, color);

                           rot = Quaternion.LookRotation(Vector3.up, Vector3.forward);
                DrawLine(rot * lastPoint + pos, rot * thisPoint + pos, color);

                           rot = Quaternion.LookRotation(Vector3.right, Vector3.forward);
                DrawLine(rot * lastPoint + pos, rot * thisPoint + pos, color);


            lastPoint = thisPoint;
            angle += 360f / segments;
        }

    }

    public static void DrawCircle_Local(Transform _T, float Radius, Color32 color)
    {
        Vector3 pos = _T.position;
        Vector3 forward = _T.up;
        Vector3 up = _T.forward;
        Vector3 Scale = new Vector3(_T.localScale.x, _T.localScale.z, _T.localScale.y) * Radius;

        int segments = 32;
        float angle = 0f;
        Quaternion rot = Quaternion.LookRotation(forward, up);
        Vector3 lastPoint = Vector3.zero;
        Vector3 thisPoint = Vector3.zero;

        for (int i = 0; i < segments + 1; i++)
        {
            thisPoint.x = Mathf.Sin(Mathf.Deg2Rad * angle) * Scale.x;
            thisPoint.y = Mathf.Cos(Mathf.Deg2Rad * angle) * Scale.y;

                DrawLine(rot * lastPoint + pos, rot * thisPoint + pos, color);
            

            lastPoint = thisPoint;
            angle += 360f / segments;
        }
    }


    public static void DrawWireCapsule(Vector3 p1, Vector3 p2, float radius)
    {
        // Special case when both points are in the same position
        if (p1 == p2)
        {
            // DrawWireSphere works only in gizmo methods
            Gizmos.DrawWireSphere(p1, radius);
            return;
        }
        using (new Handles.DrawingScope(Gizmos.color, Gizmos.matrix))
        {
            Quaternion p1Rotation = Quaternion.LookRotation(p1 - p2);
            Quaternion p2Rotation = Quaternion.LookRotation(p2 - p1);
            // Check if capsule direction is collinear to Vector.up
            float c = Vector3.Dot((p1 - p2).normalized, Vector3.up);
            if (c == 1f || c == -1f)
            {
                // Fix rotation
                p2Rotation = Quaternion.Euler(p2Rotation.eulerAngles.x, p2Rotation.eulerAngles.y + 180f, p2Rotation.eulerAngles.z);
            }

            // First side
            Handles.DrawWireArc(p1, p1Rotation * Vector3.left, p1Rotation * Vector3.down, 180f, radius);
            Handles.DrawWireArc(p1, p1Rotation * Vector3.up, p1Rotation * Vector3.left, 180f, radius);
            Handles.DrawWireDisc(p1, (p2 - p1).normalized, radius);
            // Second side
            Handles.DrawWireArc(p2, p2Rotation * Vector3.left, p2Rotation * Vector3.down, 180f, radius);
            Handles.DrawWireArc(p2, p2Rotation * Vector3.up, p2Rotation * Vector3.left, 180f, radius);
            Handles.DrawWireDisc(p2, (p1 - p2).normalized, radius);
            // Lines
            Handles.DrawLine(p1 + p1Rotation * Vector3.down * radius, p2 + p2Rotation * Vector3.down * radius);
            Handles.DrawLine(p1 + p1Rotation * Vector3.left * radius, p2 + p2Rotation * Vector3.right * radius);
            Handles.DrawLine(p1 + p1Rotation * Vector3.up * radius, p2 + p2Rotation * Vector3.up * radius);
            Handles.DrawLine(p1 + p1Rotation * Vector3.right * radius, p2 + p2Rotation * Vector3.left * radius);
        }
    }
}
