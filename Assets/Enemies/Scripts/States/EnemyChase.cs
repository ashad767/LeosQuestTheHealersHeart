using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChase : EnemyState
{
    private Transform PlayerTransform;
    private float MoveSpeed = 2f;

    public EnemyChase(Enemy enemy, EnemySM enemySM, string AnimBoolName) : base(enemy, enemySM, AnimBoolName)
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

        Vector2 direction = (PlayerTransform.position - enemy.transform.position).normalized;
        enemy.MoveEnemy(direction * MoveSpeed);

        if (enemy.IsStrike)
        {
            if(!enemy.isRanged)
                enemy.enemySM.ChangeState(enemy.AttackState);
            else
                enemy.enemySM.ChangeState(enemy.AttackRangedState);
        }

        if (!enemy.IsAggro)
            enemy.enemySM.ChangeState(enemy.WalkState);
    }
}
