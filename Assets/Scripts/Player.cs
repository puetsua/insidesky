using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(PhysicsObject))]
public class Player : MonoBehaviour
{
	public bool isAbleToJump = false;
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
	public float castingRange = 3f;

	public bool isOnLadder {
		get {
			return ladders.Count != 0;
		}
	}

	void OnTriggerEnter2D(Collider2D collider) {
		Ladder ladder = collider.GetComponent<Ladder>();
		if (ladder) {
			ladders.Add(ladder);
		}
	}
	
	void OnTriggerExit2D(Collider2D collider) {
		ladders.Remove(collider.GetComponent<Ladder>());
	}

	List<Ladder> ladders = new List<Ladder>();
}