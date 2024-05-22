using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2DPlatformer : MonoBehaviour
{
    private enum State
    {
        idle, moving, rising, falling, rolling
    };

    private State currentState;

    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private LayerMask whatIsGround;
    private Rigidbody2D theRB;
    private Animator theAnim;

    public float moveSpeed, jumpForce, jumpVelMult, hangVelThreshold,hangVelMult, fallingGravityMult;

    private bool isMoving, isFalling, isGrounded, canMove, isFacingRight;

    private float activeMoveSpeed, originalGravity;
    

    // Start is called before the first frame update
    void Start()
    {
        currentState = State.idle;

        theRB = GetComponent<Rigidbody2D>();
        theRB.constraints = RigidbodyConstraints2D.FreezeRotation;
        theAnim = GetComponent<Animator>();

        activeMoveSpeed = moveSpeed;
        originalGravity = theRB.gravityScale;

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

        if (isFalling)
        {
            theRB.gravityScale = originalGravity * fallingGravityMult;
        }
        else
        {
            theRB.gravityScale = originalGravity;
        }

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
            theRB.velocity = new Vector2(theRB.velocity.x, theRB.velocity.y + ((jumpForce + (0.5f * Time.fixedDeltaTime * -theRB.gravityScale)) / theRB.mass));
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
            theRB.velocity = new Vector2(theRB.velocity.x, theRB.velocity.y + ((jumpForce + (0.5f * Time.fixedDeltaTime * -theRB.gravityScale)) / theRB.mass));
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
        if (Math.Abs(theRB.velocity.y) < hangVelThreshold)
        {
            theRB.velocity = new Vector2(activeMoveSpeed * Input.GetAxisRaw("Horizontal") * hangVelMult, theRB.velocity.y);
        }
        else
        {
            theRB.velocity = new Vector2(activeMoveSpeed * Input.GetAxisRaw("Horizontal") * jumpVelMult, theRB.velocity.y);
        }

        if (Input.GetKeyUp(KeyCode.W) && theRB.velocity.y > 0)
        {
            theRB.velocity = new Vector2(theRB.velocity.x, theRB.velocity.y / 2);
        }

        if (theRB.velocity.y <= 0)
        {
            currentState = State.falling;
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
