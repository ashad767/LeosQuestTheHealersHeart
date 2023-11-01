using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private float bossMoveSpeed;

    public float maxHealth = 100f;
    public float currentHealth = 100f;

    private bool isAngry = false;
    private Color originalColor;
    private Color angryColor = Color.red;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        a = GetComponent<Animator>();

        originalColor = sr.color;


        StartCoroutine(follow_MC());
        StartCoroutine(shootAnim());
        StartCoroutine(dummyBossHitTester());
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
        if (!idle) 
        {
            bossMoveSpeed = isAngry ? 4.2f : 3.5f;
            transform.position = Vector2.MoveTowards(transform.position, MC.position, bossMoveSpeed * Time.deltaTime); 
        }

    }

    private IEnumerator follow_MC()
    {
        while (true)
        {
            // Boss stays idle for a random # of seconds (if angry, stays idle for less time)
            if (isAngry)
            {
                yield return new WaitForSeconds(Random.Range(1.5f, 2.4f));
            }
            else
            {
                yield return new WaitForSeconds(Random.Range(2.3f, 3.5f));
            }
            

            // Boss starts following for a random # of seconds
            idle = false;

            // I want to lunge when I'm not in the midst of shooting
            if (!shoot)
            {
                // I want the boss to lunge at the MC based on the MC's distance and direction.
                // So I first take the direction in which the boss has to lunge towards (-1 or 1, in X or Y direction)
                // Then multiply by the distance between the boss and MC. But that would be too fast,so I divided the distance by either 1.5 or 2 to make it slower
                // I then put those X & Y values as the velocity for the boss 
                float whereIsMC_X = Mathf.Sign(MC.position.x);
                float whereIsMC_Y = Mathf.Sign(MC.position.y);
                float lungeSpeedX = Mathf.Abs(MC.position.x - transform.position.x);
                float lungeSpeedY = Mathf.Abs(MC.position.y - transform.position.y);
                
                float lungeSpeedController = isAngry ? 1.5f : 2f;
                rb.velocity = new Vector2(whereIsMC_X * (lungeSpeedX / lungeSpeedController), whereIsMC_Y * (lungeSpeedY / lungeSpeedController));

                lunge = true;
                a.SetInteger("state", (int)States.lunge);
                yield return new WaitForSeconds(animLength[2].length + 0.6f);
                lunge = false;

                rb.velocity = new Vector2(0, 0); // stops boss from drifting away after lunging
            }

            // move around for a few seconds (if angry, move around for longer)
            if (isAngry)
            {
                yield return new WaitForSeconds(Random.Range(3.1f, 3.5f));  
            }
            else
            {
                yield return new WaitForSeconds(Random.Range(2.5f, 3f));
            }

            idle = true;

            // if I'm not currently shooting, then I can regenerate health
            if (!shoot)
            {
                StartCoroutine(regenFunction());
            }
        }
    }
    
    private IEnumerator regenFunction()
    {
        if (currentHealth < 50f)
        {
            if (currentHealth < 35f)
            {
                isAngry = true;
                sr.color = angryColor;
            }

            regen = true;
            currentHealth += Random.Range(10f, 20f);

            a.SetInteger("state", (int)States.regen);
            yield return new WaitForSeconds(animLength[1].length + 0.1f);
            regen = false;
        }
    }

    private IEnumerator shootAnim()
    {
        // Don't want to show shooting animation while regen animation is going on
        while (!regen)
        {
            // Wait a random # of seconds before shooting (if angry, wait less time)
            if (isAngry)
            {
                yield return new WaitForSeconds(Random.Range(2f, 2.7f));
            }
            else
            {
                yield return new WaitForSeconds(Random.Range(2.5f, 4f));
            }

            shoot = true;
            a.SetInteger("state", (int)States.shoot);

            // Instantiate the main bullet 3x rapidly when at low health
            if (isAngry)
            {
                mainBulletLoader(); 
                yield return new WaitForSeconds(animLength[0].length);

                mainBulletLoader();
                yield return new WaitForSeconds(animLength[0].length);

                mainBulletLoader();
                yield return new WaitForSeconds(animLength[0].length);
            }

            else
            {
                mainBulletLoader(); // Instantiate the main bullet
                yield return new WaitForSeconds(animLength[0].length);
            }
            shoot = false;
        }
    }
    

    private IEnumerator dummyBossHitTester()
    {
        while (true)
        {
            Color hitEffect = sr.color;

            yield return new WaitForSeconds(2.5f);
            currentHealth -= 10f;

            // When boss gets hit, I want to momentarily make the boss go slighlty transparent, then back to its original/angry color
            hitEffect.a = 0.3f;
            sr.color = hitEffect;
            yield return new WaitForSeconds(0.12f);
            sr.color = isAngry ? angryColor : originalColor;

            if (currentHealth <= 0f)
            {
                a.SetTrigger("death"); // show death animation
                rb.bodyType = RigidbodyType2D.Static;
            }
        }
    }

    void mainBulletLoader()
    {
        // Create/Instantiate a bullet prefab when shooting animation starts
        GameObject mainBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        // rotations in degrees are based on the bullet prefab being flipped (originally pointing left side)
        // flipping it on its X axis makes it point to the right and makes it easier for rotation logic
        mainBullet.GetComponent<SpriteRenderer>().flipX = true;

        Vector2 bulletDir = MC.position - mainBullet.transform.position;
        float rotation_in_Degrees = Mathf.Atan2(bulletDir.y, bulletDir.x) * Mathf.Rad2Deg; // Atan2 goes from -pi to pi. And since Atan2 returns in radians, I multiplied by Mathf.Rad2Deg to convert to degrees
        mainBullet.transform.rotation = Quaternion.Euler(0, 0, rotation_in_Degrees); // rotate on z axis
        mainBullet.GetComponent<Rigidbody2D>().velocity = bulletDir.normalized * 7.5f;
    }

    // Used in Death animation
    void DeathTest()
    {
        Destroy(transform.parent.gameObject); // Destroys boss gameobjects
    }
}