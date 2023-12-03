using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimFunctions : MonoBehaviour
{
    public Enemy enemy;

    public void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
    }

    public void OnAttack()
    {
        Debug.Log(enemy.CheckDirection() - 1);
        List<GameObject> entities = enemy.hitboxes[enemy.CheckDirection()-1].hitEnimies;

        enemy.EnemyAttack(entities);
    }

    public void OnWalkSound()
    {
        enemy.enemySM.CurrentEnemyState.AnimationTriggerEvent(enemy.WalkSound);
    }

    public void OnAttackSound()
    {
        enemy.enemySM.CurrentEnemyState.AnimationTriggerEvent(enemy.AttackSound);
    }

    public void OnDead()
    {
        Destroy(enemy.gameObject);
    }

    public void CheckAttack()
    {
        if (!enemy.IsStrike)
        {
            enemy.enemySM.ChangeState(enemy.ChaseState);
        }
    }

    public void RangedAttack()
    {
        enemy.enemySM.CurrentEnemyState.RangedAttack(enemy.gameObject.name);
    }
}
