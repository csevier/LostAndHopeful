﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

[RequireComponent(typeof(CharacterController))]

public class Character : NetworkBehaviour
{
    public float speed = 10.0f;
    public float jumpForce = 8.0f;
    public float gravity = 20.0f;
    public Vector3 moveDirection = Vector3.zero;
    private float groundDistance = 1.1f;


    public float sensitivity = 2.0f;
    public float smoothing = 5.0f;
    public float lookYLimit = 60.0f;

    private Transform childCameraTransform;
    private Vector2 mouseLook;
    private Vector2 smoothV;

    CharacterController characterController;
    public StateMachine movementSM;
    public StandingState standing;
    public JumpingState jumping;
    public FallingState falling;

    public Text debug;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        childCameraTransform = transform.Find("Main Camera");

        movementSM = new StateMachine();
        standing = new StandingState(this, movementSM);
        jumping = new JumpingState(this, movementSM);
        falling = new FallingState(this, movementSM);
        movementSM.Initialize(falling);

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public bool IsGrounded()
    {
        
        //bool isGrounded = Physics.Raycast(transform.position, Vector3.down, groundDistance);
        return characterController.isGrounded;
    }
    
    public void Move(Vector3 moveDirection)
    {
        if (isLocalPlayer)
        {
            characterController.Move(moveDirection * Time.deltaTime);
        }
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            movementSM.CurrentState.HandleInput();
            movementSM.CurrentState.LogicUpdate();
        }
        debug.text = movementSM.CurrentState.GetType().Name;
    }

    void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            movementSM.CurrentState.PhysicsUpdate();
            MouseLook();
        }
    }

    void MouseLook()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity * smoothing, sensitivity * smoothing));

        smoothV.x = Mathf.Lerp(smoothV.x, mouseDelta.x, 1f / smoothing);
        smoothV.y = Mathf.Lerp(smoothV.y, mouseDelta.y, 1f / smoothing);

        mouseLook += smoothV;
        mouseLook.y = Mathf.Clamp(mouseLook.y, -lookYLimit, lookYLimit);

        childCameraTransform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
        transform.localRotation = Quaternion.AngleAxis(mouseLook.x, transform.up);
    }

}
