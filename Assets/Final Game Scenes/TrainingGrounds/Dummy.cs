using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Dummy : Entity
{
    public GameObject floatingNumber;
    private TMP_Text tmpro;
    public override void TakeDamage(float damage)
    {
        //Debug.Log(damage);
        GameObject damageText = Instantiate(floatingNumber, transform.position, Quaternion.identity);
        tmpro = damageText.GetComponentInChildren<TMP_Text>();
        tmpro.SetText(damage.ToString());
    }
}
