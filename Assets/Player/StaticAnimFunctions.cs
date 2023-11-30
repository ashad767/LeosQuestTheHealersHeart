using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticAnimFunctions : MonoBehaviour 
{
    [SerializeField] private Player player;

    public void EndAttack()
    {
        player.stateMachine.ChangeState(player.idleState);
    }

    public void BowStateEnd()
    {
        player.IncrementBowState();
    }

    public void EndBowAttack()
    {
        Debug.Log("Ended bow");
        player.ResetBowState();
        EndAttack();
    }
}
