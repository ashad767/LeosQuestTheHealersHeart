using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDead : EnemyState
{
    public EnemyDead(Enemy enemy, EnemySM enemySM, string AnimBoolName) : base(enemy, enemySM, AnimBoolName)
    {
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
    }

    public override void AnimationTriggerEvent(AudioClip audioClip)
    {
        base.AnimationTriggerEvent(audioClip);
    }
}
