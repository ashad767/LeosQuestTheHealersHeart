using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GG_Ability : Ability
{
    public int OnAbility()
    {
        return Random.Range(0, 4);
    }
}
