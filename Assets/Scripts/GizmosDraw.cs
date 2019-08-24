using UnityEngine;

public class GizmosDraw
{
    public static readonly GizmosDraw Debug = new GizmosDraw();

    public void Circle(Vector2 center, float radius, Color color, int segments = 40)
    {

        Vector2 prev = new Vector2(
            Mathf.Cos(0f) * radius,
            Mathf.Sin(0f) * radius);
        for (int i = 0; i <= segments; i++)
        {
            Vector2 c = new Vector2(
                Mathf.Cos(Mathf.Lerp(0f, Mathf.PI * 2, i / (float)segments)) * radius,
                Mathf.Sin(Mathf.Lerp(0f, Mathf.PI * 2, i / (float)segments)) * radius);
            UnityEngine.Debug.DrawLine(center + prev, center + c, color);
            prev = c;
        }
    }
}

