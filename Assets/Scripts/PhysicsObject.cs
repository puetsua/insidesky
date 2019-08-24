using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
public sealed class PhysicsObject : MonoBehaviour {

	public Vector2 velocity = Vector2.zero;

#if UNITY_EDITOR
	new
#endif
	public Rigidbody2D rigidbody {
		get {
			if (!m_rigidbody) {
				m_rigidbody = GetComponent<Rigidbody2D>();
			}
			return m_rigidbody;
		}
	}
	Rigidbody2D m_rigidbody = null;
	
	public bool isGrounded {
		get {
			PhysicsObjectController instance = PhysicsObjectController.instance;
			Vector2 distance = instance.transform.position - transform.position;
			if (Mathf.Abs(distance.sqrMagnitude - instance.radius * instance.radius) <= instance.groundedDistanceThreshold) {
				return true;
			}
			for (int i = 0; i < collisions.Count; ++i) {
				ContactPoint2D[] contacts = collisions[i].contacts;
				for (int j = 0; j < contacts.Length; ++j) {
					Debug.DrawRay(rigidbody.position, contacts[j].normal);
					Debug.DrawRay(rigidbody.position, distance);
					if (Vector2.Angle(distance, contacts[j].normal) <= instance.groundedAngleThreshold) {
						return true;
					}
				}
			}
			return false;
		}
	}

	void OnCollisionEnter2D(Collision2D collision) {
		for (int i = 0; i < collisions.Count; ++i) {
			if (collisions[i].collider == collision.collider) {
				collisions[i] = collision;
				return;
			}
		}
		collisions.Add(collision);
	}

	void OnCollisionStay2D(Collision2D collision) {
		OnCollisionEnter2D(collision);
	}
	
	void OnCollisionExit2D(Collision2D collision) {
		for (int i = 0; i < collisions.Count; ++i) {
			if (collisions[i].collider == collision.collider) {
				collisions.RemoveAt(i);
				return;
			}
		}
	}

	List<Collision2D> collisions = new List<Collision2D>();
}