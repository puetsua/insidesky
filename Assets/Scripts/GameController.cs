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
    public float climbingSpeed = .2f;
    public Vector2 undergroundSpeed = new Vector2(3f, 4f);
    public AudioSource jumpSound;
    public float jumpingVelocity = 0.5f;
    public Player player;

    Animator playerAnim = null;

    enum PlayerState
    {
        OnGround,
        InUnderground,
        InSky
    }
    PlayerState currentState = PlayerState.OnGround;

    public void PlayerGotAbility(AbilityStar.Type type)
    {
        switch (type)
        {
            case AbilityStar.Type.Jump:
                player.isAbleToJump = true;
                mgr.onJumpGet.Invoke();
                Debug.Log("Got jump ability.");
                break;
            case AbilityStar.Type.Dig:
                player.isAbleToDig = true;
                mgr.onDigGet.Invoke();
                Debug.Log("Got dig ability.");
                break;
        }
    }

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

        if (!player.isAbleToJump && state == PlayerState.InSky)
        {
            return;
        }

        if (!player.isAbleToDig && state == PlayerState.InUnderground)
        {
            return;
        }

        int layerPlayer = LayerMask.NameToLayer("Player");
        int layerSky = LayerMask.NameToLayer("Sky");
        int layerGround = LayerMask.NameToLayer("Default");

        Physics2D.IgnoreLayerCollision(layerPlayer, layerSky, false);
        Physics2D.IgnoreLayerCollision(layerPlayer, layerGround, false);

        switch (state)
        {
            case PlayerState.OnGround:
                player.physicsObject.SetDimension(PhysicsObject.Dimension.Ground);
                Physics2D.IgnoreLayerCollision(layerPlayer, layerSky, true);
                mgr.onPlayerOnGround.Invoke();
                break;
            case PlayerState.InUnderground:
                player.physicsObject.SetDimension(PhysicsObject.Dimension.Underground);
                mgr.onPlayerUnderground.Invoke();
                break;
            case PlayerState.InSky:
                player.physicsObject.SetDimension(PhysicsObject.Dimension.Sky);
                Physics2D.IgnoreLayerCollision(layerPlayer, layerGround, true);
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
        if (player.physicsObject.isGrounded && vert < 0)
        {
            // Player wants to go underground
            SwitchPlayerState(PlayerState.InUnderground);
        }

        // Flip player when moving different direction.
        if (!player.joint.enabled)
        {
            if (hori < 0)
            {
                player.transform.localScale = new Vector3(1f, 1f, 1f);
            }
            else if (hori > 0)
            {
                player.transform.localScale = new Vector3(-1f, 1f, 1f);
            }
        }

        if (playerAnim)
        {
            // Feed information to animator
            Vector2 playerSpeed = player.physicsObject.velocity;
            playerAnim.SetFloat("Speed_X", Mathf.Abs(Vector2.Dot(playerSpeed, player.transform.up)));
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
        player.physicsObject.velocity.x = Input.GetAxis("Horizontal") * movementSpeed;
        if (player.physicsObject.isGrounded)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (player.joint.enabled)
                {
                    player.joint.enabled = false;
                    //					player.joint.connectedBody.GetComponent<PhysicsObject>().enabled = false;
                    player.joint.autoConfigureDistance = true;
                }
                else
                {
                    RaycastHit2D hit = Physics2D.Raycast(player.pivot.bounds.center, player.transform.right * -player.transform.localScale.x, player.castingRange, -1 - (1 << LayerMask.NameToLayer("Player")));
                    Debug.DrawRay(player.pivot.bounds.center, player.transform.right * -player.transform.localScale.x, Color.red, 3f);
                    if (hit)
                    {
                        PhysicsObject phyObj = hit.collider.GetComponent<PhysicsObject>();
                        if (phyObj)
                        {
                            //							phyObj.enabled = true;
                            player.joint.enabled = true;
                            player.joint.connectedBody = phyObj.rigidbody;
                            player.joint.autoConfigureDistance = false;
                        }
                    }
                }
                playerAnim.SetBool("Cast", player.joint.enabled);
            }

            if (player.isAbleToJump && !player.joint.enabled && Input.GetButtonDown("Jump"))
            {
				jumpSound.Play();
                player.physicsObject.velocity.y = jumpingVelocity;
                SwitchPlayerState(PlayerState.InSky);
            }
        }
        else // If player is not grounded
        {
            // SwitchPlayerState(PlayerState.InSky);
        }

        ClimbLadder();
    }

    void UpdateSky()
    {
        float vert = Input.GetAxis("Vertical");

        player.physicsObject.velocity.x = Input.GetAxis("Horizontal") * movementSpeed;

        if (player.physicsObject.isGroundedExceptSkyblock || vert < 0)
        {
            // touch ground or player wants go to ground world.
            SwitchPlayerState(PlayerState.OnGround);
        }

        if (player.physicsObject.isGrounded)
        {
            if (player.isAbleToJump && !player.joint.enabled && Input.GetButtonDown("Jump"))
            {
				jumpSound.Play();
                player.physicsObject.velocity.y = jumpingVelocity;
            }
        }

        ClimbLadder();
    }

    void UpdateUnderground()
    {
        float hori = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");

        Vector2 toCenter = player.transform.position - psys.world.position;
        player.transform.up = -toCenter;

        Vector2 moveVel = player.transform.up * vert * undergroundSpeed.x + player.transform.right * hori * undergroundSpeed.y;
        float radius = psys.undergroundDepth + psys.radius;

        player.physicsObject.rigidbody.velocity = moveVel;

        if (toCenter.magnitude > radius)
        {
            // Outside Border, pull player back.
            player.physicsObject.rigidbody.MovePosition(toCenter.normalized * (radius - 0.01f));
        }
        else if (toCenter.magnitude < psys.radius)
        {
            // Inside
            SwitchPlayerState(PlayerState.OnGround);
        }
    }

    void ClimbLadder()
    {
        if (player.isOnLadder)
        {
			player.physicsObject.velocity.y = climbingSpeed * Input.GetAxis("Vertical");
        }
    }
}