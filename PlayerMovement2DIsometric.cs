using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2DIsometric : MonoBehaviour
{
    public enum State
    {
        Idle, Moving
    };
    private State currentState;

    private Rigidbody2D theRB;
    private Vector2 moveInput;

    [SerializeField] private float moveSpeed;
    private float activeMoveSpeed;
    
    private bool canMove;


    private void Awake()
    {
        theRB = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        theRB.gravityScale = 0;
        currentState = State.Idle;
        activeMoveSpeed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        HandleMovement(currentState);
    }

    private void HandleMovement(State states)
    {
        switch(states)
        {
            case State.Idle:
                HandleIdleState();
                break;

            case State.Moving:
                HandleMovingState();
                break;
        }
    }

    private void HandleIdleState()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        
        if (moveInput != Vector2.zero)
        {
            currentState = State.Moving;
        }
    }

    private void HandleMovingState()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();

        theRB.velocity = moveInput * activeMoveSpeed;

        if (theRB.velocity == Vector2.zero)
        {
            currentState = State.Idle;
        }
    }
}
