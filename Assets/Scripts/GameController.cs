using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public sealed class GameController : MonoBehaviour
{
    public static GameController instance
    {
        get
        {
            if (!m_instance)
            {
                m_instance = FindObjectOfType<GameController>();
            }
            return m_instance;
        }
    }
    static GameController m_instance = null;

    GameManager mgr { get { return GameManager.instance; } }
    PhysicsSystem psys { get { return PhysicsSystem.instance; } }
    public float movementSpeed = 2f;
    public float jumpingVelocity = 0.5f;
    public PhysicsObject player;

    Animator playerAnim = null;

    enum PlayerState
    {
        OnGround,
        InUnderground,
        InSky
    }
    PlayerState currentState = PlayerState.OnGround;

    int State2IntMapping(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.InUnderground:
                return 0;
            case PlayerState.OnGround:
                return 1;
            case PlayerState.InSky:
                return 2;
        }
        return -1;
    }

    void SwitchPlayerState(PlayerState state)
    {
        if (currentState == state)
        {
            return;
        }

        switch (state)
        {
            case PlayerState.OnGround:
                player.isUnderground = false;
                mgr.onPlayerOnGround.Invoke();
                break;
            case PlayerState.InUnderground:
                player.isUnderground = true;
                mgr.onPlayerUnderground.Invoke();
                break;
            case PlayerState.InSky:
                mgr.onPlayerInSky.Invoke();
                break;
        }

        currentState = state;
        Debug.LogFormat("Changed state to {0}", currentState);
    }

    void Start()
    {
        playerAnim = player.GetComponent<Animator>();
    }

    void Update()
    {
        float hori = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");
        if (player.isGrounded && vert < 0)
        {
            // Player wants to go underground
            SwitchPlayerState(PlayerState.InUnderground);
        }

        // Flip player when moving different direction.
        if (hori < 0)
        {
            player.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (hori > 0)
        {
            player.transform.localScale = new Vector3(-1f, 1f, 1f);
        }

        if (playerAnim)
        {
            // Feed information to animator
            Vector2 playerSpeed = player.rigidbody.velocity;
            playerAnim.SetFloat("Speed_X", Vector2.Dot(playerSpeed, player.transform.up));
            playerAnim.SetFloat("Speed_Y", Vector2.Dot(playerSpeed, player.transform.right));
            playerAnim.SetInteger("State", State2IntMapping(currentState));
        }

        switch (currentState)
        {
            case PlayerState.OnGround:
                UpdateGround();
                break;
            case PlayerState.InUnderground:
                UpdateUnderground();
                break;
            case PlayerState.InSky:
                UpdateSky();
                break;
        }
    }

    void UpdateGround()
    {
        player.velocity.x = Input.GetAxis("Horizontal") * movementSpeed;
        if (player.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            player.velocity.y = jumpingVelocity;
        }
    }

    void UpdateSky()
    {
        // TODO
        player.velocity.x = Input.GetAxis("Horizontal") * movementSpeed;
        if (player.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            player.velocity.y = jumpingVelocity;
        }
    }

    void UpdateUnderground()
    {
        float hori = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");
        Vector2 distance = (Vector2)player.transform.position;
        float radius = psys.undergroundDepth + psys.radius;

        player.transform.Translate(new Vector2(hori, vert) * movementSpeed * Time.deltaTime);
        player.transform.up = -distance;

        if (distance.magnitude > radius)
        {
            // Outside Border
            player.transform.position = (Vector2)distance.normalized * radius;
            player.transform.Translate(new Vector2(hori, 0) * movementSpeed * Time.deltaTime);
        }
        else if (distance.magnitude < psys.radius)
        {
            // Inside
            SwitchPlayerState(PlayerState.OnGround);
        }
    }
}