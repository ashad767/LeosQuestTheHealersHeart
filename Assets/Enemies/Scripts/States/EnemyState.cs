using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState
{
    protected Enemy enemy;
    protected EnemySM enemySM;
    protected string AnimBoolName;

    public EnemyState(Enemy enemy, EnemySM enemySM, string AnimBoolName)
    {  
       this.enemy = enemy; 
       this.enemySM = enemySM; 
       this.AnimBoolName = AnimBoolName;   
    }

    public virtual void EnterState()
    {
        enemy.animator.SetBool(AnimBoolName, true);
    }

    public virtual void ExitState() 
    {
        enemy.animator.SetBool(AnimBoolName, false);
    }

    public virtual void FrameUpdate()
    {
    }

    public virtual void PhysicsUpdate()
    {

    }

    public virtual void AnimationTriggerEvent(AudioClip audioClip)
    {
        enemy.audioSource.clip = audioClip;
        enemy.audioSource.Play();
    }

    public virtual void RangedAttack(Rigidbody2D rb)
    {
        Rigidbody2D obj = GameObject.Instantiate(rb, enemy.transform.position, Quaternion.identity);
        obj.transform.parent = enemy.transform;
    }
}
