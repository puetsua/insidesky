using System.Collections.Generic;
using UnityEngine;

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

    public Transform world;
    public float radius = 3.0f;
    public float gravity = 0.5f;
    public float groundedDistanceThreshold = 0.01f;
    public float groundedAngleThreshold = 30f;

    [HideInInspector]
    public List<PhysicsObject> physicsObjects = new List<PhysicsObject>();

    void FixedUpdate()
    {
        foreach(var obj in physicsObjects)
        {
            UpdateObject(obj);
        }
    }

    void UpdateObject(PhysicsObject physicsObject)
    {
        Rigidbody2D rigidbody = physicsObject.rigidbody;
        Vector2 distance = (Vector2)world.position - rigidbody.position;
        rigidbody.position += ((Vector2)Vector3.Cross(distance, Vector3.forward) * physicsObject.velocity.x + distance.normalized * radius * physicsObject.velocity.y) * Time.fixedDeltaTime;
        distance = rigidbody.position - (Vector2)world.position;
        if (!physicsObject.isGrounded)
        {
            physicsObject.velocity.y -= Time.deltaTime * gravity;
        }
        rigidbody.transform.up = -distance;
        if (distance.sqrMagnitude > radius * radius)
        {
            rigidbody.position = distance.normalized * radius;
        }
    }
}