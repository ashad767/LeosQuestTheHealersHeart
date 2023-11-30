using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunState : PlayerGroundState
{
    public PlayerRunState(PlayerStateMachine playerStateMachine, Player player, string animBoolName) : base(playerStateMachine, player, animBoolName)
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
        player.SetVelocity(xInput, yInput, player.runMultiplier);

        base.Update();

        player.UseEnergy(Time.deltaTime * 5);

        if ((xInput == 0 && yInput == 0) || player.GetEnergy() <= 0)
        {
            stateMachine.ChangeState(player.idleState);
        }

        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            stateMachine.ChangeState(player.moveState);
        }
    }
}
