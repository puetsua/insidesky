using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(PhysicsObject))]
public class Player : MonoBehaviour
{
	public Collider2D pivot;
	public PhysicsObject physicsObject {
		get {
			if (!m_physicsObject) {
				m_physicsObject = GetComponent<PhysicsObject>();
			}
			return m_physicsObject;
		}
	}
	PhysicsObject m_physicsObject;
	public DistanceJoint2D joint;
}