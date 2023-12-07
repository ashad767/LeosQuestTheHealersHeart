using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDead : EnemyState
{
    private Material fade;
    private float timer = 1f;

    public EnemyDead(Enemy enemy, EnemySM enemySM, string AnimBoolName) : base(enemy, enemySM, AnimBoolName)
    {
        fade = enemy.GetComponentInChildren<SpriteRenderer>().sharedMaterial;
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
        if(enemy.IsDead)
        {
            timer -= 0.01f;
            fade.SetFloat("_Fade", timer);
            if (timer <= 0f)
            {
                fade.SetFloat("_Fade", 1f);
                GameObject.Destroy(enemy.gameObject);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        
    }

    public override void AnimationTriggerEvent(AudioClip audioClip)
    {
        base.AnimationTriggerEvent(audioClip);
    }
}
