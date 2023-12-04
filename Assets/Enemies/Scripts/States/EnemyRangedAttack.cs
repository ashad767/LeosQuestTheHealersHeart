using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangedAttack : EnemyState
{
    private Transform PlayerTransform;

    public EnemyRangedAttack(Enemy enemy, EnemySM enemySM, string AnimBoolName) : base(enemy, enemySM, AnimBoolName)
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
    }

    public override void AnimationTriggerEvent(AudioClip audioClip)
    {
        base.AnimationTriggerEvent(audioClip);
    }

    public override void RangedAttack(string name)
    {
        base.RangedAttack(name);

        Rigidbody2D proj = enemy.projectiles[name];

        //Vector2 direction = (PlayerTransform.position - enemy.transform.position).normalized;
        GameObject.Instantiate(proj, enemy.transform.position, Quaternion.identity);
        //projectile.velocity = direction * 2f;
    }
}
