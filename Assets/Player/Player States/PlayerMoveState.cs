using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerGroundState
{
    public PlayerMoveState(PlayerStateMachine playerStateMachine, Player player, string animBoolName) : base(playerStateMachine, player, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
        player.SetVelocity(0, 0, player.runMultiplier);
    }

    public override void Update()
    {
        player.SetVelocity(xInput, yInput, 1);

        base.Update();

        if (xInput == 0 && yInput == 0)
        {
            stateMachine.ChangeState(player.idleState);
        }

        else if(Input.GetKey(KeyCode.LeftShift))
        {
            stateMachine.ChangeState(player.runState);
        }
    }
}
