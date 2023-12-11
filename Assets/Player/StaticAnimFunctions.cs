using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StaticAnimFunctions : MonoBehaviour 
{
    [SerializeField] private Player player;

    public void EndAttack()
    {
        player.stateMachine.ChangeState(player.idleState);

        if (player.playerComboCounter != 2)
        {
            player.playerComboCounter++;
        }
        else
        {
            player.playerComboCounter = 0;
        }

        player.playerComboTimer = player.playerComboGrace;
    }

    public void BowStateEnd()
    {
        player.IncrementBowState();
    }

    public void EndBowAttack()
    {
        player.ResetBowState();
        EndAttack();
    }

    public void PlayerDeath()
    {
        Destroy(player.gameObject);

        SceneManager.LoadScene("Main Menu");
    }
}
