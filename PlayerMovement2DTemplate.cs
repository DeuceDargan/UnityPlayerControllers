using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2DTemplate : MonoBehaviour
{
    private enum State
    {
        idle, moving, rising, falling
    };

    private State currentState;

    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private LayerMask whatIsGround;
    private Rigidbody2D theRB;
    private Animator theAnim;

    public float moveSpeed;
    public float jumpHeight;
    public float rollSpeedMult;
    public float rollDuration;
    public float invincDuration;

    private bool isMoving;
    private bool isFalling;
    private bool isGrounded;
    private bool canMove;
    private bool isFacingRight;

    private float activeMoveSpeed;
    

    // Start is called before the first frame update
    void Start()
    {
        currentState = State.idle;

        theRB = GetComponent<Rigidbody2D>();
        theRB.constraints = RigidbodyConstraints2D.FreezeRotation;
        theAnim = GetComponent<Animator>();

        activeMoveSpeed = moveSpeed;

        canMove = true;
        isFacingRight = true;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0, (Utilities.boolToInt(isFacingRight) * 180) - 180, 0);

        isMoving = currentState != State.idle;
        isFalling = currentState == State.falling;
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, .2f, whatIsGround);

        if (canMove)
        {
            HandleMovement(currentState);
        }

        HandleAnimations();
    }

    private void HandleMovement(State states)
    {
        switch(states)
        {
            case State.idle:
                HandleIdleState();
                break;

            case State.moving:
                HandleMovingState();
                break;

            case State.rising:
                HandleRisingState();
                break;

            case State.falling:
                HandleFallingState();
                break;
        }
    }

    private void HandleIdleState()
    {
        theRB.velocity = new Vector2(activeMoveSpeed * Input.GetAxisRaw("Horizontal"), theRB.velocity.y);

        if (Input.GetKeyDown(KeyCode.W))
        {
            theRB.velocity = new Vector2(theRB.velocity.x, jumpHeight);
        }

        if (theRB.velocity != Vector2.zero)
        {
            currentState = State.moving;
        }
    }

    private void HandleMovingState()
    {
        theRB.velocity = new Vector2(activeMoveSpeed * Input.GetAxisRaw("Horizontal"), theRB.velocity.y);

        if (theRB.velocity.x > 0)
        {
            isFacingRight = true;
        }
        else if (theRB.velocity.x < 0)
        {
            isFacingRight = false;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            theRB.velocity = new Vector2(theRB.velocity.x, jumpHeight);
        }

        if (theRB.velocity.y != 0)
        {
            currentState = State.rising;
        }
        else if (theRB.velocity == Vector2.zero)
        {
            currentState = State.idle;
        }
        
    }

    private void HandleRisingState()
    {
        if (Input.GetKeyUp(KeyCode.W) && theRB.velocity.y > 0)
        {
            theRB.velocity = new Vector2(theRB.velocity.x, theRB.velocity.y / 2);
        }

        if (theRB.velocity.y < 0)
        {
            currentState = State.falling;
        }
        else if (theRB.velocity.y == 0)
        {
            currentState = State.idle;
        }
    }

    private void HandleFallingState()
    {
        if (theRB.velocity.y == 0)
        {
            currentState = State.idle;
        }
    }

    private void HandleAnimations()
    {
        theAnim.SetBool("isMoving", isMoving);
        theAnim.SetBool("isFalling", isFalling);
        theAnim.SetBool("isGrounded", isGrounded);
    }
}
