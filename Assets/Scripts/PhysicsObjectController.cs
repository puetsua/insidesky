using UnityEngine;

[DisallowMultipleComponent]
public sealed class PhysicsObjectController : MonoBehaviour {

	public static PhysicsObjectController instance {
		get {
			if (!m_instance) {
				m_instance = FindObjectOfType<PhysicsObjectController>();
			}
			return m_instance;
		}
	}
	static PhysicsObjectController m_instance = null;

	public float radius;
	public float gravity;
	public float groundedDistanceThreshold;
	public float groundedAngleThreshold;

	public PhysicsObject[] physicsObjects = new PhysicsObject[0];

	void FixedUpdate() {
		for (int i = 0; i < physicsObjects.Length; ++i) {
			UpdateObject(physicsObjects[i]);
		}
	}

	void UpdateObject(PhysicsObject physicsObject) {
		Rigidbody2D rigidbody = physicsObject.rigidbody;
		Vector2 distance = (Vector2)transform.position - rigidbody.position;
		rigidbody.position += ((Vector2)Vector3.Cross(distance, Vector3.forward) * physicsObject.velocity.x + distance.normalized * radius * physicsObject.velocity.y)* Time.fixedDeltaTime;
		distance = rigidbody.position - (Vector2)transform.position;
		if (!physicsObject.isGrounded) {
			physicsObject.velocity.y -= Time.deltaTime * gravity;
		}
		rigidbody.transform.up = -distance;
		if (distance.sqrMagnitude > radius * radius) {
			rigidbody.position = distance.normalized * radius;
		}
	}
}