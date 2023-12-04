using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

            if (player.currentWeapon is PlayerIntermediateSword)
            {
                player.AddShield(facingDirToHitbox[facingDirection].hitEnimies.Count * 1);
                Debug.Log("Add Shield " + facingDirToHitbox[facingDirection].hitEnimies.Count);
            }

            if(player.currentWeapon is PlayerAdvancedSword)
            {
                playerSword.Attack(facingDirToHitbox[facingDirection].hitEnimies, Mathf.Min(1, player.swordLevel) * player.playerComboCounter * player.playerComboScalingDamage);
            }
            else
            {
                playerSword.Attack(facingDirToHitbox[facingDirection].hitEnimies, 0);
            }
        }

        player.attackTimer = player.playerAttackCooldown;
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
