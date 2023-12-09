using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class EnemyRangedAttack : EnemyState
{
    private Transform PlayerTransform;
    private NavMeshAgent agent;

    public EnemyRangedAttack(Enemy enemy, EnemySM enemySM, string AnimBoolName) : base(enemy, enemySM, AnimBoolName)
    {
        PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        agent = enemy.GetComponent<NavMeshAgent>();
    }

    public override void EnterState()
    {
        base.EnterState();
        enemy.RB.constraints = RigidbodyConstraints2D.FreezeAll;
        agent.speed = 0f;
    }

    public override void ExitState()
    {
        base.ExitState();
        enemy.RB.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        enemy.MoveEnemy(Vector2.zero);
        if(enemy.ability != null)
            enemy.ability.OnAbility();
        //Debug.Log("ability");

    }

    public override void AnimationTriggerEvent(AudioClip audioClip)
    {
        base.AnimationTriggerEvent(audioClip);
    }

    public override void RangedAttack(Rigidbody2D rb)
    {
        base.RangedAttack(rb);
    }
}
