using UnityEngine;

[DisallowMultipleComponent]
public sealed class GameController : MonoBehaviour
{
    public float movementSpeed = 2f;
    public float jumpingVelocity = 0.5f;
    public PhysicsObject player;
    public bool isGrounded;

    void Update()
    {
        player.velocity.x = Input.GetAxis("Horizontal") * movementSpeed;
        if (player.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            player.velocity.y = jumpingVelocity;
        }
        isGrounded = player.isGrounded;
    }
}