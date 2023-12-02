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
}
