using UnityEngine;

[DisallowMultipleComponent]
public sealed class GameController : MonoBehaviour
{
    PhysicsSystem psys { get { return PhysicsSystem.instance; } }
    public float movementSpeed = 2f;
    public float jumpingVelocity = 0.5f;
    public PhysicsObject player;
    public bool isGrounded;

    void Update()
    {
        float vert = Input.GetAxis("Vertical");
        if (player.isGrounded && vert < 0)
        {
            // Player wants to go underground
            player.isUnderground = true;
        }

        if (player.isUnderground)
        {
            UpdateUnderground();
        }
        else
        {
            UpdateGround();
        }
    }

    void UpdateGround()
    {
        player.velocity.x = Input.GetAxis("Horizontal") * movementSpeed;
        if (player.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            player.velocity.y = jumpingVelocity;
        }
        isGrounded = player.isGrounded;
    }

    void UpdateUnderground()
    {
        float hori = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");
        Vector2 distance = (Vector2)player.transform.position;
        float radius = psys.undergroundDepth + psys.radius;

        player.transform.Translate(new Vector2(hori, vert) * movementSpeed * Time.deltaTime);
        player.transform.up = -distance;

        float sqrRad = radius * radius;

        if (distance.sqrMagnitude != sqrRad)
        {
            if (distance.sqrMagnitude > sqrRad)
            {
                player.transform.position = distance.normalized * radius;
            }
        }
    }
}