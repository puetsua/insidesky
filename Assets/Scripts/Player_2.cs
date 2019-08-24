using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_2 : MonoBehaviour
{
    GameManager mgr { get { return GameManager.instance; } }
    
    public Rigidbody2D rigidbody;
    public float gravity = .5f;
	public float speed = 2f;
    public float m_radius = 15f;

    // Start is called before the first frame update
    void Start()
    {
          //m_radius = mgr.worldRadius;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float hori = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");
        Vector2 distance = (Vector2)transform.position;

        transform.Translate(new Vector2(hori, vert) * speed * Time.deltaTime);
        rigidbody.transform.up = -distance;

        float sqrRad = m_radius * m_radius;
        //rigidbody.position -= distance.normalized * m_radius * Time.deltaTime;

        if (distance.sqrMagnitude != sqrRad)
        {
            if (distance.sqrMagnitude > sqrRad)
            {
                rigidbody.position = distance.normalized * m_radius;
            }
            else
            {
                
            }
        }

        
    }
}
