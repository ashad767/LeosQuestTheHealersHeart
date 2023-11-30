using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;

public class PlayerBow : PlayerWeapon
{
    public GameObject arrow;
    public Camera camera;

    public PlayerBow(Sprite _weaponImage, string _weaponName) : base(_weaponImage, _weaponName)
    {
    }
    public void Attack(Transform transform, Vector3 target)
    {
        target.z = 1;

        Vector3 mousePos = camera.ScreenToWorldPoint(target);



        var tarX = mousePos.x - transform.position.x;
        var tarY = mousePos.y - transform.position.y;

        float angle = Mathf.Atan2(tarY, tarX) * Mathf.Rad2Deg;
        var finalAngle = Quaternion.Euler(new Vector3(0, 0, angle));

        GameObject arr = Instantiate(arrow, transform.position, finalAngle);
        arr.transform.parent = null;
      
    }
}
