using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExpertSword : PlayerAdvancedSword
{
    public GameObject effect;
    public PlayerExpertSword(Sprite _weaponImage, string _weaponName) : base(_weaponImage, _weaponName)
    {
    }

    public override void Attack(List<GameObject> hitTargets, int extraDamage)
    {
        base.Attack(hitTargets, extraDamage);

        foreach (GameObject target in hitTargets)
        {
            if (target != null)
            {
                Entity current = target.GetComponentInChildren<Entity>();

                if (current != null && !target.CompareTag("Player"))
                {
                        GameObject addedEffect = Instantiate(effect);
                        addedEffect.transform.SetParent(current.transform);
                }
            }
        }
    }
}


