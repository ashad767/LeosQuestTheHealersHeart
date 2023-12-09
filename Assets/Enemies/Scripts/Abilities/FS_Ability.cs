using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FS_Ability : Ability
{
    private Enemy enemy;
    public Rigidbody2D FSWall;
    private Vector2 dir;
    public AudioClip blockPlace;
    private AudioSource audioSource;

    private void Start()
    {
        enemy = GetComponent<Enemy>();
        audioSource = GetComponent<AudioSource>();
    }

    public override void OnAbility()
    {
        if(cooldown <= 0 ) 
        {

            Vector3 direction =  (PlayerTransform.position - enemy.transform.position);

            Vector3 newPos = PlayerTransform.position + 1.2f * direction.normalized;


            /*if (direction.x > 0f)
            {
                if (direction.y > 0f)
                {
                    dir = new Vector2(0.25f, 0.25f);
                }
                else
                    dir = new Vector2(0.25f, -0.25f);

            }
            else
            {
                if (direction.y > 0f)
                {
                    dir = new Vector2(-0.25f, 0.25f);
                }
                else
                    dir = new Vector2(-0.25f, -0.25f);
            }

            newPos.x += dir.x;
            newPos.y += dir.y;*/

            Rigidbody2D rb = GameObject.Instantiate(FSWall, newPos, Quaternion.identity);
            audioSource.PlayOneShot(blockPlace, 0.6f);

            //float hyp = Mathf.Sqrt((direction.x * direction.x) + (direction.y * direction.y));

            float rot = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;

            /*rot *= (180f / Mathf.PI);
 

            if (Mathf.Abs(rot) < 10f || Mathf.Abs(rot) > 80f)
            {
                Debug.Log("cos");
                rot = Mathf.Asin((direction.x / hyp));
                rot *= (180f / Mathf.PI);
            }*/

            /*if (direction.x > 0f)
            {
                if (direction.y > 0f)
                {
                    rot = Mathf.Asin((direction.x / hyp));
                    
                }
                else
                {
                    rot = Mathf.Asin((direction.y / hyp));

                    if ((rot *= (180f / Mathf.PI)) > 70f)
                        rot = Mathf.Asin((direction.x / hyp));
                }
            }
            else
            {
                if (direction.y > 0f)
                {
                    rot = Mathf.Asin((direction.y / hyp));

                    if((rot *= (180f / Mathf.PI)) > 70f)
                        rot = Mathf.Asin((direction.x / hyp));
                }
                else
                    rot = Mathf.Asin((direction.x / hyp));
            }*/

            //rot = Mathf.Max(Mathf.Asin((direction.y / hyp)), Mathf.Asin((direction.x / hyp)));
            //rot *= (180f / Mathf.PI);

            
            /*if (Mathf.Abs(rot) < 5f)
                rot = 85f * (rot/rot);
            else if (Mathf.Abs(rot) > 85f)
                rot = 5f * (rot / rot)*/
            

            //if (direction.x > 0)
            //{
             //   rot *= -1;
            //}

            

            rb.transform.Rotate(0, 0, rot+90);
            
            cooldown = 3f;
        }
    }
}