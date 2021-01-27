using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingState : AirborneState
{
    public JumpingState(Character character, StateMachine stateMachine) : base(character, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        character.jump = false;
        character.moveDirection.y = character.jumpForce;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (character.moveDirection.y <= 0.0f)
        {
            stateMachine.ChangeState(character.falling);
        }
    }
}
