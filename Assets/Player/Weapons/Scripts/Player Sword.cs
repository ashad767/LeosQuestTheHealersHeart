using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSword : PlayerWeapon
{
    public PlayerSword(Sprite _weaponImage, string _weaponName) : base(_weaponImage, _weaponName)
    {
    }

    public virtual void Attack(List<GameObject> hitTargets)
    {
        foreach (GameObject target in hitTargets)
        {
            Debug.Log("Hit " + target.name + " with " + weaponName);
        }
    }
}
