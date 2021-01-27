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
        float curSpeedX = speed * Input.GetAxis("Vertical");
        float curSpeedY = character.moveDirection.y;
        float curSpeedZ = speed * Input.GetAxis("Horizontal");
        character.moveDirection = (forward * curSpeedX) + (right * curSpeedZ) + (up * curSpeedY);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        character.Move(character.moveDirection);
    }

}