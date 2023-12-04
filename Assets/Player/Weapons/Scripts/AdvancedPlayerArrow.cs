using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedPlayerArrow : PlayerArrow
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        Transform hitObject = collision.GetComponentInParent<Transform>();

        if (collision.CompareTag("Enemy") && hitObject != null)
        {
            //hitObject.AddForce(rb.velocity * 2);
            hitObject.position += new Vector3(arrowVelocity.x, arrowVelocity.y, 0) / 20;
        }
    }
}
