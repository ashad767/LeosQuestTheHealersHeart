using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SR_Ability : Ability
{
    public float timer = 2f;
    public bool OnFire = false;
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


        /*if (OnFire)
        {
            //Debug.Log("on fire");
            timer -= Time.deltaTime;
            player.TakeDamage(Time.deltaTime * 3f);
            if (timer <= 0)
            {
                OnFire = false;
                timer = 5f;
                cooldown = 5f;
                collided = false;
            }
        }*/
        if (cooldown <= 0)
        {
            chance = Random.Range(0, 4);

            if (chance == 0)
            {
                cooldown = 5f;
                collided = false;
            }
            else
                collided = false;
        }
    }
}
