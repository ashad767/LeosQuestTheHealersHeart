using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class L2BossMovement : Entity
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator a;
    [SerializeField] private AnimationClip[] animLength;
    [SerializeField] private Transform MC;
    [SerializeField] private Slider healthBar;
    [SerializeField] private GameObject audioManagerToBeUsedInDestroyBullet;

    #region Prefabs
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject shadowClonePrefab;
    [SerializeField] private GameObject angryChargePrefab;
    #endregion

    #region Audio
    [SerializeField] AudioSource lungeAudio;
    [SerializeField] AudioSource shootBulletAudio;
    [SerializeField] AudioSource regenAudio;
    [SerializeField] AudioSource death;
    #endregion

    #region Animation states & bools
    private enum States { idle, lunge, shoot, regen };

    public bool idle = true;
    private bool lunge = false;
    private bool shoot = false;
    private bool regen = false;
    private bool dead = false;
    #endregion

    #region Boss when Angry
    public bool isAngry = false;
    private Color originalColor;
    private Color angryColor = Color.red;
    #endregion

    private float bossMoveSpeed;
    public float currentHealth;
    private bool collidingWithplayer = false;

    protected override void Start()
    {
        base.Start(); // Simply sets "CurrentHealth = maxHealth;"

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        a = GetComponent<Animator>();

        originalColor = sr.color;

        StartCoroutine(follow_MC());
        StartCoroutine(shootAnim());
    }

    // Update is called once per frame
    protected override void Update()
    {
        if(MC != null)
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
                bossMoveSpeed = isAngry ? 2.5f : 2f;
                transform.position = Vector2.MoveTowards(transform.position, MC.position, bossMoveSpeed * Time.deltaTime);
            }
        }

        // If player dies, just go idle
        else
        {
            a.SetInteger("state", (int)States.idle);
        }

        // Boss death
        if (GetHealth() <= 0 && !dead)
        {
            StartCoroutine(BossDeath());
        }
    }

    private IEnumerator follow_MC()
    {
        while (MC != null && true)
        {
            // if I'm not currently shooting, then I can regenerate health (if boss is in low health)
            if (!shoot)
            {
                StartCoroutine(regenFunction());
            }

            // Boss stays idle for a random # of seconds (if angry, stays idle for less time)
            if (isAngry)
            {
                yield return new WaitForSeconds(Random.Range(1.5f, 2f));
            }
            else
            {
                yield return new WaitForSeconds(Random.Range(2.4f, 3f));
            }
            

            // Boss starts following for a random # of seconds
            idle = false;

            // I want to lunge when I'm not in the midst of shooting
            if (!shoot)
            {
                StartCoroutine(lungeAnim());
            }

            // Move around
            yield return new WaitForSeconds(Random.Range(2.5f, 3f));

            // Go back to being idle
            idle = true;

            // Create/Instantiate a shadow prefab when idle
            bool createShadow = Random.Range(0f, 1f) <= 0.75f; // if a float is picked randomly and it's less than 0.75f, then create shadow clone, else don't
            if (createShadow)
            {
                GameObject shadow = Instantiate(shadowClonePrefab, transform.position, Quaternion.identity);
                ShadowClone sc = shadow.GetComponent<ShadowClone>();
                sc.MC = MC.gameObject;
                sc.isBossAngry = isAngry;
            }
        }
    }

    private IEnumerator lungeAnim()
    {
        // I want the boss to lunge at the MC based on the MC's distance and direction.
        // So I first take the direction in which the boss has to lunge towards (-1 or 1, in X or Y direction)
        // Then multiply by the distance between the boss and MC. But that would be too fast,so I divided the distance by either 1.5 or 1.7 to make it slower
        // I then put those X & Y values as the velocity for the boss 
        float whereIsMC_X = Mathf.Sign(MC.position.x);
        float whereIsMC_Y = Mathf.Sign(MC.position.y);
        float lungeSpeedX = Mathf.Abs(MC.position.x - transform.position.x);
        float lungeSpeedY = Mathf.Abs(MC.position.y - transform.position.y);

        float lungeSpeedController = isAngry ? 1.5f : 1.7f;
        rb.velocity = new Vector2(whereIsMC_X * (lungeSpeedX / lungeSpeedController), whereIsMC_Y * (lungeSpeedY / lungeSpeedController));

        lunge = true;
        a.SetInteger("state", (int)States.lunge);
        lungeAudio.Play();
        yield return new WaitForSeconds(animLength[2].length + 0.6f);
        lunge = false;

        rb.velocity = new Vector2(0, 0); // stops boss from drifting away after lunging
    }

    private IEnumerator shootAnim()
    {
        while (MC != null && true)
        {
            // Wait a random # of seconds before shooting (if angry, wait less time)
            if (isAngry)
            {
                yield return new WaitForSeconds(Random.Range(2f, 2.5f));
            }
            else
            {
                yield return new WaitForSeconds(Random.Range(2f, 3f));
            }

            // Don't want to show shooting animation while regen animation is going on
            if (!regen)
            {
                shoot = true;
                a.SetInteger("state", (int)States.shoot);

                // Instantiate the main bullet 3x rapidly when at low health
                if (isAngry)
                {
                    for(int i = 0; i < 3; i++)
                    {
                        mainBulletLoader();
                        shootBulletAudio.Play();
                        yield return new WaitForSeconds(animLength[0].length);
                    }
                }

                else
                {
                    mainBulletLoader(); // Instantiate the main bullet
                    shootBulletAudio.Play();
                    yield return new WaitForSeconds(animLength[0].length);
                }

                shoot = false;
            }
        }
    }

    void mainBulletLoader()
    {
        // Create/Instantiate a bullet prefab when shooting animation starts
        GameObject mainBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        DestroyBullet db = mainBullet.GetComponent<DestroyBullet>();
        db.audioManager = audioManagerToBeUsedInDestroyBullet;

        // rotations in degrees are based on the bullet prefab being flipped (originally pointing left side)
        // flipping it on its X axis makes it point to the right and makes it easier for rotation logic
        mainBullet.GetComponent<SpriteRenderer>().flipX = true;

        Vector2 bulletDir = MC.position - mainBullet.transform.position;
        float rotation_in_Degrees = Mathf.Atan2(bulletDir.y, bulletDir.x) * Mathf.Rad2Deg; // Atan2 goes from -pi to pi (counter-clockwise starting from the left side). And since Atan2 returns in radians, I multiplied by Mathf.Rad2Deg to convert to degrees
        mainBullet.transform.rotation = Quaternion.Euler(0, 0, rotation_in_Degrees); // rotate on z axis

        float bulletSpeed = isAngry ? 15.5f : 11f;
        mainBullet.GetComponent<Rigidbody2D>().velocity = bulletDir.normalized * bulletSpeed;
    }

    private IEnumerator regenFunction()
    {
        if (GetHealth() < 50f)
        {
            if (GetHealth() < 35f)
            {
                GameObject charge = Instantiate(angryChargePrefab, transform.position, Quaternion.identity);
                charge.transform.SetParent(transform);
                Destroy(charge, animLength[4].length);
                isAngry = true;
                sr.color = angryColor;
            }

            regen = true;
            a.SetInteger("state", (int)States.regen);
            regenAudio.Play();
            CurrentHealth += isAngry ? Random.Range(15f, 27f) : Random.Range(10f, 20f);
            
            yield return new WaitForSeconds(animLength[1].length + 0.1f);
            regen = false;
        }
    }


    #region Continuous damage logic
    // Same steps as 'OnTriggerEnter2D()'
    private void OnTriggerStay2D(Collider2D collision)
    {
        OnTriggerEnter2D(collision);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!collidingWithplayer)
            {
                collision.gameObject.GetComponent<Player>().TakeDamage(2f);
                collidingWithplayer = true; // Used as a flag in case of repeated inflicted damage on player
                StartCoroutine(waitForNextDamageTick());
            }
        }
    }

    private IEnumerator waitForNextDamageTick()
    {
        yield return new WaitForSeconds(0.5f);
        collidingWithplayer = false; // Reset the attack flag to let the next attack audio & animation play (if any)
    }
    #endregion


    private IEnumerator BossDeath()
    {
        dead = true;
        rb.bodyType = RigidbodyType2D.Static;
        a.SetTrigger("death"); // show death animation
        death.Play();

        destroyPrefabs();

        yield return new WaitForSeconds(death.clip.length);
        Destroy(gameObject); // Destroys boss gameobject
    }

    private void destroyPrefabs()
    {
        // Find all active shadow clone prefabs in the scene and destroy them
        GameObject[] shadowClonesToDestroy = GameObject.FindGameObjectsWithTag("shadow_clone");
        GameObject[] bulletsToDestroy = GameObject.FindGameObjectsWithTag("wizardBullet");

        foreach (GameObject shadowClone in shadowClonesToDestroy)
        {
            Destroy(shadowClone);
        }

        foreach (GameObject bullet in bulletsToDestroy)
        {
            Destroy(bullet);
        }
    }

    private void OnDestroy()
    {
        Destroy(healthBar.gameObject);
    }
}