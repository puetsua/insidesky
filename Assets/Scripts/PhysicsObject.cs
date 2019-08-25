using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
public sealed class PhysicsObject : MonoBehaviour
{
    PhysicsSystem psys { get { return PhysicsSystem.instance; } }
    public Vector2 velocity = Vector2.zero;

    [HideInInspector]
    public bool isUnderground = false;

#if UNITY_EDITOR
	void OnEnable() {}

    new
#endif
    public Rigidbody2D rigidbody
    {
        get
        {
            if (!m_rigidbody)
            {
                m_rigidbody = GetComponent<Rigidbody2D>();
            }
            return m_rigidbody;
        }
    }
    Rigidbody2D m_rigidbody = null;

    public bool isGrounded
    {
        get
        {
            Vector2 distance = psys.transform.position - transform.position;
            if (psys.radius - distance.magnitude <= psys.groundedDistanceThreshold)
            {
                return true;
            }
            for (int i = 0; i < collisions.Count; ++i)
            {
                ContactPoint2D[] contacts = collisions[i].contacts;
                for (int j = 0; j < contacts.Length; ++j)
                {
                    if (Vector2.Angle(distance, contacts[j].normal) <= psys.groundedAngleThreshold)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        for (int i = 0; i < collisions.Count; ++i)
        {
            if (collisions[i].collider == collision.collider)
            {
                collisions[i] = collision;
                return;
            }
        }
        collisions.Add(collision);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        OnCollisionEnter2D(collision);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        for (int i = 0; i < collisions.Count; ++i)
        {
            if (collisions[i].collider == collision.collider)
            {
                collisions.RemoveAt(i);
                return;
            }
        }
    }

    void Awake()
    {
        psys.physicsObjects.Add(this);
    }

    List<Collision2D> collisions = new List<Collision2D>();
}