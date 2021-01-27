using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirborneState : State
{
    protected float speed;

    public AirborneState(Character character, StateMachine stateMachine) : base(character, stateMachine)
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
        /* no input for now
        Vector3 forward = character.transform.TransformDirection(Vector3.forward);
        Vector3 right = character.transform.TransformDirection(Vector3.right);
        Vector3 up = character.transform.TransformDirection(Vector3.up);
        float curSpeedX = moveDirection.x;
        //float curSpeedX = speed * Input.GetAxis("Vertical");
        float curSpeedY = moveDirection.y;
        float curSpeedZ = moveDirection.z;
        //float curSpeedZ = speed * Input.GetAxis("Horizontal");
        moveDirection = (forward * curSpeedX) + (right * curSpeedZ) + (up * curSpeedY);
        */
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        // apply gravity
        character.moveDirection.y -= character.gravity * Time.deltaTime;
        character.Move(character.moveDirection);
    }
}
