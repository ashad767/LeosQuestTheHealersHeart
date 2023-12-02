using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedPlayerArrow : PlayerArrow
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        Rigidbody2D hitObject = collision.GetComponentInParent<Rigidbody2D>();

        if (collision.CompareTag("Enemy") && hitObject != null)
        {
            hitObject.AddForce(rb.velocity * 2);
        }

        base.OnTriggerEnter2D(collision);
    }
}
