using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCircle : MonoBehaviour
{
    GameManager mgr { get { return GameManager.instance; } }
    PhysicsSystem psys { get { return PhysicsSystem.instance; } }

    LineRenderer line = null;

    // Start is called before the first frame update
    void Start()
    {
        GameObject container = this.gameObject;
        float radius = psys.radius;
        float lineWidth = 0.05f;

        var segments = 360;
        line = container.GetComponent<LineRenderer>();
        if (line)
        {
            line.useWorldSpace = false;
            line.startWidth = lineWidth;
            line.endWidth = lineWidth;
            line.positionCount = segments + 1;

            var pointCount = segments + 1; // add extra point to make startpoint and endpoint the same to close the circle
            var points = new Vector3[pointCount];

            for (int i = 0; i < pointCount; i++)
            {
                var rad = Mathf.Deg2Rad * (i * 360f / segments);
                points[i] = new Vector3(Mathf.Sin(rad) * radius, Mathf.Cos(rad) * radius, 0);
            }

            line.SetPositions(points);
        }
    }
}
