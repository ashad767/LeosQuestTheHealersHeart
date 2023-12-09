using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyChase : EnemyState
{
    private Transform PlayerTransform;
    private float MoveSpeed = 1.5f;
    private NavMeshAgent agent;

    public EnemyChase(Enemy enemy, EnemySM enemySM, string AnimBoolName) : base(enemy, enemySM, AnimBoolName)
    {
        PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        agent = enemy.GetComponent<NavMeshAgent>();
    }

    public override void EnterState()
    {
        base.EnterState();
        agent.speed = 1.5f;
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        Vector2 direction = (PlayerTransform.position - enemy.transform.position).normalized;
        enemy.MoveEnemy(direction * agent.speed);
        agent.destination = PlayerTransform.position;
        //enemy.animator.SetFloat("xInput", enemy.RB.velocity.x);
        //enemy.animator.SetFloat("yInput", enemy.RB.velocity.y);

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
