using UnityEngine;

public class NewBehaviourScript1 : MonoBehaviour {

#if UNITY_EDITOR
	new
#endif
	public Rigidbody2D rigidbody;
	public float gravity = .5f;
	public float speed = 2f;
	public float groundedThreshold = .005f;
	public float groundedTimeout = .03f;
	public bool isGrounded = false;

	void Start() {
		m_radius = ((Vector2)transform.position - rigidbody.position).magnitude;
		m_previousPosition = rigidbody.position;
	}

    void FixedUpdate() {
		float hori = Input.GetAxis("Horizontal");
		Vector2 distance = (Vector2)transform.position - rigidbody.position;
		rigidbody.position += (Vector2)Vector3.Cross(distance, Vector3.forward) * hori * speed * Time.fixedDeltaTime;
		distance = rigidbody.position - (Vector2)transform.position;
		float sqrtDist = distance.sqrMagnitude;
		float sqrRad = m_radius * m_radius;
		rigidbody.position -= distance.normalized * m_radius * Time.deltaTime * m_jumpVelocity;
		rigidbody.transform.up = -distance;
		if (sqrtDist != sqrRad) {
			if (sqrtDist > sqrRad) {
				rigidbody.position = distance.normalized * m_radius;
			} else {
				rigidbody.position += distance.normalized * m_radius * Time.deltaTime * gravity;
			}
		}
		if (Input.GetKeyDown(KeyCode.Space)) {
			m_jumpVelocity = 1f;
		}
		if (m_jumpVelocity > 0f) {
			m_jumpVelocity -= gravity * Time.fixedDeltaTime;
		}
		Debug.DrawRay(rigidbody.position, distance);
		Debug.DrawLine(rigidbody.position, m_previousPosition);
		float grounded;
		if (sqrtDist > 0f) {
			grounded = Vector3.Project(m_previousPosition - rigidbody.position, distance).sqrMagnitude / (Time.fixedDeltaTime * Time.fixedDeltaTime);
		} else {
			grounded = 0f;
		}
		if (grounded > groundedThreshold) {
			if (m_groundedTime > groundedTimeout) {
				isGrounded = false;
			} else {
				m_groundedTime += Time.fixedDeltaTime;
			}
		} else {
			m_groundedTime = 0f;
			isGrounded = true;
		}
		m_previousPosition = rigidbody.position;
	}

	float m_radius = 3f;
	float m_jumpVelocity = 0f;
	Vector2 m_previousPosition;
	float m_groundedTime = 0f;
}