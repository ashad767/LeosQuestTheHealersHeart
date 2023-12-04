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
        player.UseEnergy(player.playerDashCost);
        player.invincibleTimer = 1;

        if (player.anim.GetFloat("xVelocity") == 0 && player.anim.GetFloat("yVelocity") == 0)
        {
            facingDirection = player.idleState.facingDirection;
            if(facingDirection == 1)
            {
                player.anim.SetFloat("yVelocity", 1);
            }
            else if (facingDirection == 2)
            {
                player.anim.SetFloat("xVelocity", 1);
            }
            else if (facingDirection == 3)
            {
                player.anim.SetFloat("yVelocity", -1);
            }
            else if (facingDirection == 4)
            {
                player.anim.SetFloat("xVelocity", -1);
            }
        }
    }

    public override void Exit()
    {
        player.dashTimer = player.playerDashCooldown;
        player.SetVelocity(0, 0, 0);
        player.invincibleTimer = 0;
        base.Exit();
    }

    public override void Update()
    {
        player.invincibleTimer = 1;
        stateTimer -= Time.deltaTime;
        mousePosition = Input.mousePosition;

        player.SetVelocity(player.anim.GetFloat("xVelocity"), player.anim.GetFloat("yVelocity"), player.playerDashSpeed);

        if (stateTimer < 0)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
