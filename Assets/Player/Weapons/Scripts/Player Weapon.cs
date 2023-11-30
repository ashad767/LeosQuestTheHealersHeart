using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    #region Weapon Info

    [Header("Weapon Information")]
    [SerializeField] protected Sprite weaponImage;
    [SerializeField] protected string weaponName;

    #endregion

    public PlayerWeapon(Sprite _weaponImage, string _weaponName)
    {
        this.weaponImage = _weaponImage;
        this.weaponName = _weaponName;
    }

    public virtual void PrintName()
    {
        Debug.Log(weaponName);
    }

    public virtual void Attack()
    {
        Debug.Log("Atack " + weaponName);
    }
}
