using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMagicState : PlayerState
{
    public PlayerMagicState(PlayerStateMachine playerStateMachine, Player player, string animBoolName) : base(playerStateMachine, player, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.SetVelocity(0, 0, player.runMultiplier);
        if(player.magicLevel == 3)
        {
            player.invincibleTimer = 2;
        }
    }

    public override void Exit()
    {
        base.Exit();

        PlayerMagic playerMagic = player.currentWeapon as PlayerMagic;
        playerMagic.Attack(player.transform);

        player.healTimer = player.playerHealCooldown;
    }

    public override void Update()
    {
        base.Update();
    }
}
