using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWalk : EnemyState
{
    private Vector3 target;
    private Vector3 direction;
    private BoxCollider2D bc;

    public EnemyWalk(Enemy enemy, EnemySM enemySM, string AnimBoolName) : base(enemy, enemySM, AnimBoolName)
    {
        bc = enemy.GetComponentInChildren<BoxCollider2D>();
    }

    public override void EnterState()
    {
        base.EnterState();
        target = GetRandomPoint();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        if (enemy.IsAggro)
            enemy.enemySM.ChangeState(enemy.ChaseState);

        direction = (target - enemy.transform.position).normalized;

        enemy.MoveEnemy(direction * enemy.MovementSpeed);

        if((enemy.transform.position - target).sqrMagnitude < 0.01f)
        {
            target = GetRandomPoint();
        }

        OnTriggerEnter2D(bc);
    }

    public override void AnimationTriggerEvent(AudioClip audioClip)
    {
        base.AnimationTriggerEvent(audioClip);
    }

    private Vector3 GetRandomPoint()
    {
        return enemy.transform.position + (Vector3)UnityEngine.Random.insideUnitCircle * enemy.MovementRange;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("WALL"))
        {
            target = GetRandomPoint();
        }
    }
}
