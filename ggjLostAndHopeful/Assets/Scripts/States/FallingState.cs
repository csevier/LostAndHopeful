using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingState : AirborneState
{
    public FallingState(Character character, StateMachine stateMachine) : base(character, stateMachine)
    {
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if(character.IsGrounded())
        {
            stateMachine.ChangeState(character.standing);
        }
    }
}
