using UnityEngine;

public class Player : MonoBehaviour {

#if UNITY_EDITOR
	new
#endif
	public Rigidbody2D rigidbody;
	public float gravity = .5f;
	public float speed = 2f;

	void Start() {
		m_radius = ((Vector2)transform.position - rigidbody.position).magnitude;
	}

    void FixedUpdate() {
		float hori = Input.GetAxis("Horizontal");
		Vector2 distance = (Vector2)transform.position - rigidbody.position;
		rigidbody.position += (Vector2)Vector3.Cross(distance, Vector3.forward) * hori * speed * Time.fixedDeltaTime;
		distance = rigidbody.position - (Vector2)transform.position;
		float sqrRad = m_radius * m_radius;
		rigidbody.position -= distance.normalized * m_radius * Time.deltaTime * m_jumpVelocity;
		if (distance.sqrMagnitude != sqrRad) {
			if (distance.sqrMagnitude > sqrRad) {
				rigidbody.position = distance.normalized * m_radius;
			} else {
				rigidbody.position += distance.normalized * m_radius * Time.deltaTime * gravity;
			}
		}
		rigidbody.transform.up = -distance;
		if (Input.GetKeyDown(KeyCode.Space)) {
			m_jumpVelocity = 1f;
		}
		if (m_jumpVelocity > 0f) {
			m_jumpVelocity -= gravity * Time.fixedDeltaTime;
		}
	}

	float m_radius = 3f;
	[SerializeField] float m_jumpVelocity = 0f;
}