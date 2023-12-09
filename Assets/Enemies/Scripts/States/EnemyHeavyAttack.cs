using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHeavyAttack : EnemyState
{
    protected Transform PlayerTransform;

    public EnemyHeavyAttack(Enemy enemy, EnemySM enemySM, string AnimBoolName) : base(enemy, enemySM, AnimBoolName)
    {
        PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        enemy.MoveEnemy(Vector2.zero);

       // if (enemy.ability != null)
         //   enemy.ability.OnAbility();
    }

    public override void AnimationTriggerEvent(AudioClip audioClip)
    {
        base.AnimationTriggerEvent(audioClip);
    }
}
