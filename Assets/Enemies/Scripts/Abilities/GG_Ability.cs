using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GG_Ability : Ability
{
    public bool isBlock = false;

    public override void OnAbility()
    {
        if (Random.Range(0, 4) == 0)
        {
            isBlock = true;
            Debug.Log("blocked");
        }
    }
}
