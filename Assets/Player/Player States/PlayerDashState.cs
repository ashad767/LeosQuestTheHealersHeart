using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    public PlayerDashState(PlayerStateMachine playerStateMachine, Player player, string animBoolName) : base(playerStateMachine, player, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = player.playerDashDuration;
    }

    public override void Exit()
    {
        player.SetVelocity(0, 0, 0);
        base.Exit();
    }

    public override void Update()
    {
        stateTimer -= Time.deltaTime;
        mousePosition = Input.mousePosition;

        player.SetVelocity(player.anim.GetFloat("xVelocity"), player.anim.GetFloat("yVelocity"), player.playerDashSpeed);

        if (stateTimer < 0)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
