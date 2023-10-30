using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator a;
    [SerializeField] private AnimationClip[] animLength;
    [SerializeField] private Transform MC;
    [SerializeField] private GameObject bulletPrefab;

    private enum States { idle, lunge, shoot, regen };

    public bool idle = true;
    private bool lunge = false;
    private bool shoot = false;
    private bool regen = false;

    private float health = 100f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        a = GetComponent<Animator>();

        StartCoroutine(follow_MC());
        StartCoroutine(shootAnim());
        StartCoroutine(testingHealth());
    }


    // Update is called once per frame
    void Update()
    {
        // Flip boss sprite on its X axis depending on if the MC is left or right of the boss
        sr.flipX = MC.position.x > transform.position.x;  

        if (!shoot && !regen && !lunge)
        {
            a.SetInteger("state", (int)States.idle);
        }

        // Make the boss move towards MC when MC is NOT idle
        if (!idle) { transform.position = Vector3.MoveTowards(transform.position, MC.position, 2f * Time.deltaTime); }

    }

    private IEnumerator follow_MC()
    {
        while(true)
        {
            // Boss stays idle for a random # of seconds
            yield return new WaitForSeconds(Random.Range(3f, 6.5f));

            // Boss starts following for a random # of seconds
            idle = false;

            // I want the boss to lunge at the MC based on the MC's distance and direction.
            // So I first take the direction in which the boss has to lunge towards (-1 or 1, in X or Y direction)
            // Then multiply by the distance between the boss and MC. But that would be too fast,so I divided the distance by 2 to make it slower
            // I then put those X & Y values as the velocity for the boss 
            float whereIsMC_X = Mathf.Sign(MC.position.x);
            float whereIsMC_Y = Mathf.Sign(MC.position.y);
            float lungeSpeedX = Mathf.Abs(MC.position.x - transform.position.x);
            float lungeSpeedY = Mathf.Abs(MC.position.y - transform.position.y);
            rb.velocity = new Vector2(whereIsMC_X * (lungeSpeedX / 2), whereIsMC_Y * (lungeSpeedY / 2));

            lunge = true;
            a.SetInteger("state", (int)States.lunge);
            yield return new WaitForSeconds(animLength[2].length + 0.7f);
            lunge = false;

            rb.velocity = new Vector2(0,0); // stops boss from drifting away after lunging

            yield return new WaitForSeconds(Random.Range(4.5f, 8f));

            idle = true;

            if(health < 50f)
            {
                health += Random.Range(10f, 20f);

                regen = true;
                a.SetInteger("state", (int)States.regen);
                yield return new WaitForSeconds(animLength[1].length + 0.1f);
                regen = false;
            }
        }
    }

    private IEnumerator shootAnim()
    {
        // Don't want to show shooting animation while regen animation is going on
        while(!regen)
        {
            // Wait a random # of seconds before shooting
            yield return new WaitForSeconds(Random.Range(2.5f, 5f));

            shoot = true;
            a.SetInteger("state", (int)States.shoot);

            // Create/Instantiate a bullet prefab when shooting animation starts
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

            // rotations in degrees are based on the bullet prefab being flipped (originally pointing left side)
            // flipping it on its X axis makes it point to the right and makes it easier for rotation logic
            bullet.GetComponent<SpriteRenderer>().flipX = true; 

            Vector2 bulletDir = MC.position - bullet.transform.position;
            float rotation_in_Degrees = Mathf.Atan2(bulletDir.y, bulletDir.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.Euler(0,0, rotation_in_Degrees);
            bullet.GetComponent<Rigidbody2D>().velocity = bulletDir.normalized * 6f;

            yield return new WaitForSeconds(animLength[0].length);
            shoot = false;
        }
    }

    private IEnumerator testingHealth()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2f, 4f));
            health -= 5;

            if(health < 0f)
            {
                sr.enabled = false;
            }
        }
    }
}