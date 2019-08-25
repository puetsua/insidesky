using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class PhysicsSystem : MonoBehaviour
{
    public static PhysicsSystem instance
    {
        get
        {
            if (!m_instance)
            {
                m_instance = FindObjectOfType<PhysicsSystem>();
            }
            return m_instance;
        }
    }
    static PhysicsSystem m_instance = null;
    GameManager mgr { get { return GameManager.instance; } }
    GameController ctrler { get { return GameController.instance; } }

    public Transform world;
    public float radius = 3.0f;
    public float undergroundDepth = 4.0f;
    public float gravity = 0.5f;
    public float groundedDistanceThreshold = 0.01f;
    public float groundedAngleThreshold = 30f;

    [NonSerialized]
    public List<PhysicsObject> physicsObjects = new List<PhysicsObject>();

    public Vector2 TowardCenter(Transform obj)
    {
        return (obj.position - world.position).normalized;
    }

    public float Dist2Center(Transform obj)
    {
        return Vector2.Distance(world.position, obj.position);
    }

    void Start()
    {
        mgr.DrawCircle(this.gameObject, radius, 0.1f);
        mgr.DrawCircle(this.gameObject, radius + undergroundDepth, 0.1f);
    }

    void FixedUpdate()
    {
        foreach (var obj in physicsObjects)
        {
            if (obj.isUnderground)
            {
                UpdateUnderground(obj);
            }
            else
            {
                UpdateGround(obj);
            }
        }
    }

    void UpdateGround(PhysicsObject physicsObject)
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
        else if (physicsObject.velocity.y <= 0)
        {
            physicsObject.velocity.y = 0;
        }
        rigidbody.transform.up = -direcion;
        if (distance > radius)
        {
            rigidbody.position = direcion.normalized * radius;
        }
        if(ctrler.player.isUnderground){
            
            physicsObject.GetComponent<Renderer>().enabled = false;
        }else{
            
            physicsObject.GetComponent<Renderer>().enabled = true;
        }
    }

    void UpdateUnderground(PhysicsObject physicsObject)
    {
        if (ctrler.player == physicsObject)
        {
            return;
        }

        Rigidbody2D rigid = physicsObject.rigidbody;
        Vector2 dir = (Vector2)world.position - rigid.position;
        float centerDist = dir.magnitude;
        dir.Normalize();
        physicsObject.velocity = Vector2.zero;
        rigid.transform.up = -dir; // Facing outward.

        // Do not go inside.
        if (centerDist < radius)
        {
            rigid.MovePosition(-dir * radius);
        }
        if(ctrler.player.isUnderground){
            
            physicsObject.GetComponent<Renderer>().enabled = true;
        }else{
            
            physicsObject.GetComponent<Renderer>().enabled = false;
        }


    }

    void OnDrawGizmos()
    {
        GizmosDraw.Debug.Circle(world.transform.position, radius, Color.red);
        GizmosDraw.Debug.Circle(world.transform.position, radius + undergroundDepth, Color.magenta);
    }
}