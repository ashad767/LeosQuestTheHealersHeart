using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : EnemyState
{
    private Transform PlayerTransform;

    public EnemyAttack(Enemy enemy, EnemySM enemySM, string AnimBoolName) : base(enemy, enemySM, AnimBoolName)
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
        enemy.RB.constraints = RigidbodyConstraints2D.FreezePositionY;
        enemy.RB.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (!enemy.IsStrike)
        {
            enemy.enemySM.ChangeState(enemy.ChaseState);
            enemy.RB.constraints = RigidbodyConstraints2D.None;
            enemy.RB.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    public override void AnimationTriggerEvent(AudioClip audioClip)
    {
        base.AnimationTriggerEvent(audioClip);
    }
}
