using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class HealFieldIntermediate : HealField
{
    public int heal;

    private float healTimer = 1;
    private Player player;
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (collision.CompareTag("Player"))
        {
            player = collision.gameObject.GetComponent<Player>();
        }
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);

        if (collision.CompareTag("Player"))
        {
            player = null;
        }
    }

    protected override void Update()
    {
        base.Update();

        healTimer += Time.deltaTime;

        if(player != null && healTimer >= 1)
        {
            player.Heal(heal);
            healTimer = 0;
        }
    }
}
