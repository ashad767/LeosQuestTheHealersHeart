using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : EnemyState
{
    protected Transform PlayerTransform;
    private NavMeshAgent agent;

    public EnemyAttack(Enemy enemy, EnemySM enemySM, string AnimBoolName) : base(enemy, enemySM, AnimBoolName)
    {
        PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        agent = enemy.GetComponent<NavMeshAgent>();
    }

    public override void EnterState()
    {
        base.EnterState();
        //enemy.RB.constraints = RigidbodyConstraints2D.FreezePositionY;
        //enemy.RB.constraints = RigidbodyConstraints2D.FreezePositionX;
        enemy.RB.constraints = RigidbodyConstraints2D.FreezeAll;
        agent.speed = 0f;
        //enemy.RB.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public override void ExitState()
    {
        base.ExitState();
       // enemy.RB.constraints = RigidbodyConstraints2D.None;
        enemy.RB.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        enemy.MoveEnemy(Vector2.zero);

        if(enemy.ability != null && !(enemy.ability is Zombie_Ability))
        {
            enemy.ability.OnAbility();
        }
    }

    public override void AnimationTriggerEvent(AudioClip audioClip)
    {
        base.AnimationTriggerEvent(audioClip);
    }
}
