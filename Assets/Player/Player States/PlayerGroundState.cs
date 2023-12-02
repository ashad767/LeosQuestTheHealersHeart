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

        InputAndAnims();
        CheckStateTransitionInputs();

        if ((player.playerComboTimer <= 0 && player.playerComboCounter > 0))
            player.playerComboCounter = 0;
    }

    private void CheckStateTransitionInputs()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (player.currentWeapon is PlayerMagic)
            {
                if(player.healTimer <= 0)
                    stateMachine.ChangeState(player.magicState);
            }

            else
            {
                if(player.attackTimer <= 0)
                    stateMachine.ChangeState(player.attackState);
            }
        }

        else if (Input.GetKeyDown(KeyCode.LeftControl) && player.CanDash())
        {
            stateMachine.ChangeState(player.dashState);
        }
    }

    private void InputAndAnims()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        player.anim.SetFloat("xVelocity", xInput);
        player.anim.SetFloat("yVelocity", yInput);

        facingDirection = GetFacingDirection();
        player.anim.SetFloat("FacingDirection", facingDirection);
        player.attackState.facingDirection = facingDirection;
    }
}
