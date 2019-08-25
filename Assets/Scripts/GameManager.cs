using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public sealed class GameManager : MonoBehaviour
{
    public static GameManager instance
    {
        get
        {
            if (!m_instance)
            {
                m_instance = FindObjectOfType<GameManager>();
            }
            return m_instance;
        }
    }
    static GameManager m_instance = null;

    public MusicController musicCtrl { get { return MusicController.instance; } }
    public PhysicsSystem phyCtrl { get { return PhysicsSystem.instance; } }

    public bool isDebug = false;
    public LineRenderer debugLineRenderer = null;
    public UnityEvent onPlayerInSky = new UnityEvent();
    public UnityEvent onPlayerOnGround = new UnityEvent();
    public UnityEvent onPlayerUnderground = new UnityEvent();

    public void DrawCircle(GameObject container, float radius, float lineWidth)
    {
        var segments = 360;
        if (debugLineRenderer && isDebug)
        {
            LineRenderer line = Instantiate(debugLineRenderer, container.transform);

            line.useWorldSpace = false;
            line.startWidth = lineWidth;
            line.endWidth = lineWidth;
            line.positionCount = segments + 1;

            var pointCount = segments + 1; // add extra point to make startpoint and endpoint the same to close the circle
            var points = new Vector3[pointCount];

            for (int i = 0; i < pointCount; i++)
            {
                var rad = Mathf.Deg2Rad * (i * 360f / segments);
                points[i] = new Vector3(Mathf.Sin(rad) * radius, Mathf.Cos(rad) * radius, 0f);
            }

            line.SetPositions(points);
        }
    }
}