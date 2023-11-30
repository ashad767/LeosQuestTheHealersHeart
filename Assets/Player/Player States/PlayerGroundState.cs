using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundState : PlayerState
{
    public PlayerGroundState(PlayerStateMachine playerStateMachine, Player player, string animBoolName) : base(playerStateMachine, player, animBoolName)
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

    public override void Update()
    {
        base.Update();

        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        player.anim.SetFloat("xVelocity", xInput);
        player.anim.SetFloat("yVelocity", yInput);

        facingDirection = GetFacingDirection();
        player.anim.SetFloat("FacingDirection", facingDirection);
        player.attackState.facingDirection = facingDirection;


        if (Input.GetMouseButtonDown(0))
        {
            stateMachine.ChangeState(player.attackState);
            Debug.Log("Attacking");
        }

        else if(Input.GetKeyDown(KeyCode.LeftControl) && player.CanDash())
        {
            player.dashTimer = player.playerDashCooldown;
            stateMachine.ChangeState(player.dashState);
        }
    }
}
