using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyWalk : EnemyState
{
    public Vector3 target;
    private Vector3 direction;
    private NavMeshAgent agent;
    private BoxCollider2D bc;
    public float timer = 0f;

    public EnemyWalk(Enemy enemy, EnemySM enemySM, string AnimBoolName) : base(enemy, enemySM, AnimBoolName)
    {
        agent = enemy.GetComponent<NavMeshAgent>();
        bc = enemy.GetComponentInChildren<BoxCollider2D>();
    }

    public override void EnterState()
    {
        base.EnterState();
        target = GetRandomPoint();
        agent.speed = 1f;
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        timer += Time.deltaTime;
        if (enemy.IsAggro)
            enemy.enemySM.ChangeState(enemy.ChaseState);

        direction = (target - enemy.transform.position).normalized;
        agent.destination = target;

        enemy.MoveEnemy(direction * agent.speed);

        if((enemy.transform.position - target).sqrMagnitude < 0.01f)
        {
            target = GetRandomPoint();
        }

        else if(timer >= 2f)
        {
            timer = 0f;
            target = GetRandomPoint();
        }
    }

    public override void AnimationTriggerEvent(AudioClip audioClip)
    {
        base.AnimationTriggerEvent(audioClip);
    }

    public Vector3 GetRandomPoint()
    {
        return enemy.transform.position + (Vector3)UnityEngine.Random.insideUnitCircle * enemy.MovementRange;
    }
}
