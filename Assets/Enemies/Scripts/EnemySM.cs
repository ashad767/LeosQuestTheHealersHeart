using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySM
{
    public EnemyState CurrentEnemyState { get; private set; }

    public void Initialize(EnemyState state) 
    {
        CurrentEnemyState = state;
        CurrentEnemyState.EnterState();
    }

    public void ChangeState(EnemyState state)
    {
        CurrentEnemyState.ExitState();
        CurrentEnemyState = state;
        CurrentEnemyState.EnterState();
    }
}
