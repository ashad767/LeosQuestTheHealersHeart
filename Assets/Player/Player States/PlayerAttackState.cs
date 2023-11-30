using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerState
{
    private Dictionary<int, HitBox> facingDirToHitbox = new Dictionary<int, HitBox>();

    public PlayerAttackState(PlayerStateMachine playerStateMachine, Player player, string animBoolName) : base(playerStateMachine, player, animBoolName)
    {
        facingDirToHitbox.Add(1, player.swordUpHitBox);
        facingDirToHitbox.Add(2, player.swordRightHitBox);
        facingDirToHitbox.Add(3, player.swordDownHitBox);
        facingDirToHitbox.Add(4, player.swordLeftHitBox);
    }

    public override void Enter()
    {
        base.Enter();
        player.SetVelocity(0, 0, player.runMultiplier);
    }

    public override void Exit()
    {
        base.Exit();

        if (player.currentWeapon is PlayerSword)
        {
            PlayerSword playerSword = player.currentWeapon as PlayerSword;
            playerSword.Attack(facingDirToHitbox[facingDirection].hitEnimies);
        }

        else if (player.currentWeapon is PlayerMagic)
        {
            PlayerMagic playerMagic = player.currentWeapon as PlayerMagic;
            playerMagic.Attack(player.transform);
        }
    }

    public override void Update()
    {
        base.Update();
        BowAttackChecks();
    }

    private void BowAttackChecks()
    {
        if (player.currentWeapon is PlayerBow)
        {
            PlayerBow playerBow = player.currentWeapon as PlayerBow;

            if (player.bowChargeState == 1)
            {
                facingDirection = GetFacingDirection();
                player.anim.SetFloat("FacingDirection", facingDirection);
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (player.bowChargeState == 1)
                {
                    playerBow.Attack(player.transform, Input.mousePosition);
                    player.IncrementBowState();
                }

                else if (player.bowChargeState == 0)
                {
                    stateMachine.ChangeState(player.idleState);
                }
            }
        }
    }
}
