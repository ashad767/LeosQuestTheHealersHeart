using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMagic : PlayerWeapon
{
    public GameObject healField;

    public PlayerMagic(Sprite _weaponImage, string _weaponName) : base(_weaponImage, _weaponName)
    {
    }

    public void Attack(Transform position)
    {
        GameObject field = Instantiate(healField, position);
        field.transform.parent = null;
    }
}
