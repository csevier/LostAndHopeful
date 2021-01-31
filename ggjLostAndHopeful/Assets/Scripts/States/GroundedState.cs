using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedState : State
{
    protected float speed;

    public GroundedState(Character character, StateMachine stateMachine) : base(character, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        character.animator.Play("Idle");
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void HandleInput()
    {
        base.HandleInput();
        Vector3 forward = character.transform.TransformDirection(Vector3.forward);
        Vector3 right = character.transform.TransformDirection(Vector3.right);
        Vector3 up = character.transform.TransformDirection(Vector3.up);
        Vector2 curInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        float curSpeedX = speed * curInput.y;
        float curSpeedY = character.moveDirection.y;
        float curSpeedZ = speed * curInput.x;
        Vector3 moveDir = (forward * curSpeedX) + (right * curSpeedZ) + (up * curSpeedY);
        Debug.Log(curInput.magnitude);
        if (curInput.magnitude > 0.5) {

            character.animator.Play("Run");
        }
        else
        {
            character.animator.Play("Idle");
        }
        character.moveDirection = moveDir; 

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        character.Move(character.moveDirection);
    }

}