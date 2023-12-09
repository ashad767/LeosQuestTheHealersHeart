using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyDead : EnemyState
{
    private Material fade;
    private float timer = 1f;
    private NavMeshAgent agent;

    public EnemyDead(Enemy enemy, EnemySM enemySM, string AnimBoolName) : base(enemy, enemySM, AnimBoolName)
    {
        fade = enemy.GetComponentInChildren<SpriteRenderer>().sharedMaterial;
        agent = enemy.GetComponent<NavMeshAgent>();
    }

    public override void EnterState()
    {
        base.EnterState();
        agent.speed = 0f;
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
                Rigidbody2D rb = GameObject.Instantiate(enemy.coin, enemy.transform.position, Quaternion.identity);
                rb.gameObject.GetComponent<Coin>().value = enemy.value;
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
