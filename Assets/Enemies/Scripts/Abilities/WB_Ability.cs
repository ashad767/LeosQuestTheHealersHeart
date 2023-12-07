using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WB_Ability : Ability
{
    private float chance;
    public float timer = 2f;
    public bool stunned = false;
    public Rigidbody2D pRB;
    public Animator playerAnim;
    private float origSpeed;

    private void Start()
    {
        pRB = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        playerAnim = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Animator>();
    }


    public override void OnAbility()
    {
        if (!collided)
        {
            return;
        }

        /*if (stunned)
        {
            Debug.Log("stunned");
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                stunned = false;
                //pRB.constraints = RigidbodyConstraints2D.None;
                pRB.constraints = RigidbodyConstraints2D.FreezeRotation;
                timer = 2f;
                cooldown = 5f;
                collided = false;
            }
        }*/
        if (cooldown <= 0)
        {
            chance = Random.Range(0, 4);

            if (chance == 0 && !player.IsRooted)
            {
                player.IsRooted = true;
                pRB.constraints = RigidbodyConstraints2D.FreezeAll;
                cooldown = 5f;
                collided = false;
                // pRB.constraints = RigidbodyConstraints2D.FreezePositionX;
            }
            else
                collided = false;
        }
    }
}
