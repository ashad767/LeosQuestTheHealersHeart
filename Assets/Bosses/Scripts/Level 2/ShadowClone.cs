using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;

public class ShadowClone : Entity
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator a;
    [SerializeField] private AnimationClip[] shadowAnimLength;
    [SerializeField] private GameObject shadowPoofPrefab;

    private bool isWalkingSlow = true;
    private float shadowMoveSpeed;
    private float walkTimer;

    private bool collidingWithplayer = false;
    private float timer = 0f;

    private bool dead= false;

    // Getting these values from Wizard.cs when I instantiate a shadow clone.
    // Took me a long time to figure out that you can't just drag in and drop gameObjects from hierarchy window into prefab scripts in Inspector window.
    // Have to get a reference/s from the script that instantiates this prefab and assign values from there to use it here.
    public bool isBossAngry;
    public GameObject MC;

    #region Audio
    [SerializeField] AudioSource shadowWhisper;
    [SerializeField] AudioSource shadowPoofExplode;
    [SerializeField] AudioSource shadowPoofMagic;
    #endregion

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start(); // Simply sets "CurrentHealth = maxHealth;"

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        a = GetComponent<Animator>();

        // Initialize timers
        walkTimer = Random.Range(3f, 6f); // Adjust the initial walk duration
        SetNewSpeed();

        StartCoroutine(lungeAnim());
        shadowWhisper.Play();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if(MC != null)
        {
            // Flip boss sprite on its X axis depending on if the MC is left or right of the boss
            sr.flipX = MC.transform.position.x > transform.position.x;

            // Update timer
            walkTimer -= Time.deltaTime;

            // Check if the walk duration is over
            if (walkTimer <= 0f)
            {
                // Reset the timer
                walkTimer = Random.Range(2f, 7f);
                SetNewSpeed();
            }

            transform.position = Vector2.MoveTowards(transform.position, MC.transform.position, shadowMoveSpeed * Time.deltaTime);
        }

        else
        {
            Destroy(gameObject);
        }

        // Shadow death
        if (GetHealth() <= 0 && !dead)
        {
            StartCoroutine(ShadowDeath());
        }
    }

    private IEnumerator lungeAnim()
    {
        // I want the boss to lunge at the MC based on the MC's distance and direction.
        // So I first take the direction in which the boss has to lunge towards (-1 or 1, in X or Y direction)
        // Then multiply by the distance between the boss and MC. But that would be too fast,so I divided the distance by either 1.5 or 2 to make it slower
        // I then put those X & Y values as the velocity for the boss 
        float whereIsMC_X = Mathf.Sign(MC.transform.position.x);
        float whereIsMC_Y = Mathf.Sign(MC.transform.position.y);
        float lungeSpeedX = Mathf.Abs(MC.transform.position.x - transform.position.x);
        float lungeSpeedY = Mathf.Abs(MC.transform.position.y - transform.position.y);

        float lungeSpeedController = isBossAngry ? Random.Range(1.2f, 1.4f) : Random.Range(1.4f, 1.5f);
        rb.velocity = new Vector2(whereIsMC_X * (lungeSpeedX / lungeSpeedController), whereIsMC_Y * (lungeSpeedY / lungeSpeedController));

        yield return new WaitForSeconds(shadowAnimLength[0].length + 0.6f);

        a.SetBool("idle", true);

        rb.velocity = new Vector2(0, 0); // stops boss from drifting away after lunging
    }

    // Function to set a new speed based on the current pace
    private void SetNewSpeed()
    {
        if (isWalkingSlow)
        {
            shadowMoveSpeed = Random.Range(2.7f, 3f);
        }
        else
        {
            shadowMoveSpeed = Random.Range(3.3f, 4f);
        }

        isWalkingSlow = !isWalkingSlow; // Switch the pace for next call
    }

    #region Continuous damage logic
    // Same steps as 'OnTriggerEnter2D()'
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            timer += Time.deltaTime;

            if (timer >= 1.2f)
            {
                timer = 0f;
                collision.gameObject.GetComponent<Player>().TakeDamage(2f);
                CurrentHealth = 0f;
            }

            OnTriggerEnter2D(collision);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!collidingWithplayer)
            {
                collision.gameObject.GetComponent<Player>().TakeDamage(1f);
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

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            timer = 0f;
        }
    }
    #endregion

    private IEnumerator ShadowDeath()
    {
        dead = true;
        rb.bodyType = RigidbodyType2D.Static;
        a.SetTrigger("shadowDeath"); // show death animation

        GameObject shadow = Instantiate(shadowPoofPrefab, transform.position, Quaternion.identity);
        shadowPoofExplode.Play();
        shadowPoofMagic.Play();

        yield return new WaitForSeconds(shadowPoofMagic.clip.length);
        Destroy(shadow);
        Destroy(gameObject); // Destroys boss gameobject
    }
}
