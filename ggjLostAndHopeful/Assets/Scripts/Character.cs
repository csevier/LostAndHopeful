using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]

public class Character : MonoBehaviour
{
    public float speed = 10.0f;
    public float jumpForce = 8.0f;
    public float gravity = 20.0f;
    public Vector3 moveDirection = Vector3.zero;
    private float groundDistance = 1.1f;

    CharacterController characterController;
    public StateMachine movementSM;
    public StandingState standing;
    public JumpingState jumping;
    public FallingState falling;

    public Text debug;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        movementSM = new StateMachine();
        standing = new StandingState(this, movementSM);
        jumping = new JumpingState(this, movementSM);
        falling = new FallingState(this, movementSM);
        movementSM.Initialize(falling);

        // Lock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundDistance);
    }
    
    public void Move(Vector3 moveDirection)
    {
        characterController.Move(moveDirection * Time.deltaTime);
    }

    void Update()
    {
        movementSM.CurrentState.HandleInput();
        movementSM.CurrentState.LogicUpdate();
        debug.text = movementSM.CurrentState.GetType().Name;
    }

    void FixedUpdate()
    {
        movementSM.CurrentState.PhysicsUpdate();
    }
}
