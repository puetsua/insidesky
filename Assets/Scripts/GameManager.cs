using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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
    public GameController gameCtrl { get { return GameController.instance; } }

    public bool isDebug = false;
    public LineRenderer debugLineRenderer = null;
    public float winRadius = 3f;

    public UnityEvent onPlayerInSky = new UnityEvent();
    public UnityEvent onPlayerOnGround = new UnityEvent();
    public UnityEvent onPlayerUnderground = new UnityEvent();
    public UnityEvent onJumpGet = new UnityEvent();
    public UnityEvent onDigGet = new UnityEvent();
    public UnityEvent onWinning = new UnityEvent();

    float nextSceneTimer = 0;
    float nextSceneTime = 5f;

    public void DrawCircle(GameObject container, float radius, float lineWidth, Color color)
    {
        if (color == default(Color))
        {
            color = Color.white;
        }

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

    void Start()
    {
        if (isDebug)
        {
            DrawCircle(this.gameObject, winRadius, 0.1f, Color.yellow);
        }

        onPlayerOnGround.AddListener(() =>
        {
            GameObject[] groundObjs = GameObject.FindGameObjectsWithTag("Ground");
            GameObject[] skyObjs = GameObject.FindGameObjectsWithTag("Sky");
            foreach (var obj in groundObjs)
            {
                obj.GetComponent<Renderer>().enabled = true;
            }
            foreach (var obj in skyObjs)
            {
                obj.GetComponent<Renderer>().enabled = false;
            }
        });
        onPlayerInSky.AddListener(() =>
        {
            GameObject[] groundObjs = GameObject.FindGameObjectsWithTag("Ground");
            GameObject[] skyObjs = GameObject.FindGameObjectsWithTag("Sky");
            foreach (var obj in groundObjs)
            {
                obj.GetComponent<Renderer>().enabled = false;
            }
            foreach (var obj in skyObjs)
            {
                obj.GetComponent<Renderer>().enabled = true;
            }
        });

        // Ground, hide sky.
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Sky");
        foreach (var obj in objs)
        {
            obj.GetComponent<Renderer>().enabled = false;
        }
    }

    void Update()
    {
        if (nextSceneTimer > 0) // Player has won the game.
        {
            nextSceneTimer -= Time.unscaledDeltaTime;
            if (nextSceneTimer <= 0)
            {
                Scene scene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(scene.name, LoadSceneMode.Single);
                Time.timeScale = 1f;
            }
        }
        else if (nextSceneTimer == 0)
        {
            if (Vector2.Distance(phyCtrl.world.position, gameCtrl.player.transform.position) < winRadius)
            {
                Debug.Log("Player WIN!");
                Time.timeScale = 0.1f;
                nextSceneTimer = nextSceneTime;
                onWinning.Invoke();
            }
        }
    }

    void OnDrawGizmos()
    {
        GizmosDraw.Debug.Circle(transform.position, winRadius, Color.yellow);
    }
}