using System;
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

    [NonSerialized]
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
		rigidbody.velocity = Vector2.zero;
        Vector2 direcion = (Vector2)world.position - rigidbody.position;
		float distance = direcion.magnitude;
		rigidbody.position += ((Vector2)Vector3.Cross(direcion, Vector3.forward) * physicsObject.velocity.x * (distance / radius) + direcion.normalized * radius * physicsObject.velocity.y) * Time.fixedDeltaTime;
		direcion = rigidbody.position - (Vector2)world.position;
        if (!physicsObject.isGrounded)
        {
            physicsObject.velocity.y -= Time.deltaTime * gravity;
        }
		rigidbody.transform.up = -direcion;
		if (distance > radius)
        {
			rigidbody.position = direcion.normalized * radius;
        }
    }
}