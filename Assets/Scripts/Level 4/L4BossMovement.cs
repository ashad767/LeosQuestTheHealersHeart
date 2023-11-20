using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L4BossMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator a;
    [SerializeField] private AnimationClip[] animLength;
    [SerializeField] private Transform MC;
    
    // Animation states
    private enum States { idle, walk, attack, rocksFall };

    public bool idle = true;
    private bool walk = false;
    private bool attack = false;
    private bool rocksFall = false;
    private bool dead = false;

    // Prefabs
    [SerializeField] private GameObject startFireBallRainPrefab;
    GameObject startFireBallRainPrefabInstance; // used in fireBallRain() coroutine

    [SerializeField] private rocksFallManager rFM;

    // Health
    public float currentHealth = 100f;
    public float maxHealth = 100f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        a = GetComponent<Animator>();

        StartCoroutine(follow_MC());
        StartCoroutine(fireBallRain());
        //StartCoroutine(dummyBossHitTester());

    }

    // Update is called once per frame
    void Update()
    {
        // Flip boss sprite on its X axis depending on if the MC is left or right of the boss
        sr.flipX = MC.position.x > transform.position.x;

        if (idle && !attack && !rocksFall)
        {
            a.SetInteger("state", (int)States.idle);
        }

        // Make the boss move towards MC when MC is NOT idle
        if (!idle && !attack && !dead)
        {
            transform.position = Vector2.MoveTowards(transform.position, MC.position, 5f * Time.deltaTime);
        }
    }

    private IEnumerator follow_MC()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            idle = false;

            walk = true;
            // walk animation gets activated after attack animation ends
            if (!attack)
            {
                a.SetInteger("state", (int)States.walk);
            }
            yield return new WaitForSeconds(2f);
            walk = false;

            idle = true;
            yield return new WaitForSeconds(0.2f);

            if (!attack)
            {
                StartCoroutine(rocksFallFunc());
                yield return new WaitForSeconds(animLength[1].length);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            attack = true;
            a.SetInteger("state", (int)States.attack);
            StartCoroutine(attackFunc());
        }
    }
    // Same steps as 'OnTriggerEnter2D()'
    private void OnTriggerStay2D(Collider2D collision)
    {
        OnTriggerEnter2D(collision);
    }
    private IEnumerator attackFunc()
    {
        // swingAudio.Play();
        yield return new WaitForSeconds(animLength[0].length);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(LetAttackAnimationFinish());
        }
    }
    private IEnumerator LetAttackAnimationFinish()
    {
        // if player quickly enters and exits boss' box collider, it first triggers 'OnTriggerEnter2D()' which plays the attack animation, but I have to add this delay when exiting or else the attack animation would get interrupted by the walk/idle animation
        yield return new WaitForSeconds(animLength[0].length / 1.4f);

        attack = false;

        // If the boss was walking before the attack, transition back to walking animation
        if (walk)
        {
            a.SetInteger("state", (int)States.walk);
        }
    }

    private IEnumerator fireBallRain()
    {
        bool startFireBallRainIsActive = false;
        
        yield return new WaitForSeconds(Random.Range(5f, 10f)); // Wait 5-10 sec before giving the chance to start a fireball rain

        while (true)
        {
            if (!startFireBallRainIsActive)
            {
                startFireBallRainIsActive = true;
                GameObject startFireBallRain = Instantiate(startFireBallRainPrefab, transform.position, Quaternion.identity);
                startFireBallRain.transform.SetParent(transform);

                startFireBallRainPrefabInstance = GameObject.FindWithTag("startFireBallRain");
            }

            if (startFireBallRainPrefabInstance == null)
            {
                startFireBallRainIsActive = false;
                yield return new WaitForSeconds(Random.Range(5f, 10f));
            }

            // Since I'm not yielding a return value in any of the if-statements, if I don't put this line, the game will freeze as it would not update to the next game frame.
            yield return null;

        }
    }

    private IEnumerator rocksFallFunc()
    {
        rocksFall = true;

        a.SetInteger("state", (int)States.rocksFall);
        yield return new WaitForSeconds(animLength[1].length);
        
        rFM.spawnRocks();

        rocksFall = false;
    }


    private IEnumerator dummyBossHitTester()
    {
        while (true)
        {
            Color originalColor = sr.color;
            Color hitEffect = sr.color;

            yield return new WaitForSeconds(2f);
            currentHealth -= 10f;

            // When boss gets hit, I want to momentarily make the boss go slighlty transparent, then back to its original/angry color
            hitEffect.a = 0.2f;
            sr.color = hitEffect;
            yield return new WaitForSeconds(0.1f);
            sr.color = originalColor;

            if (currentHealth <= 0f)
            {
                dead = true;
                GetComponent<CircleCollider2D>().enabled = false;
                rb.bodyType = RigidbodyType2D.Static;
                a.SetTrigger("death"); // show death animation
                //deathAudio.Play();

                //yield return new WaitForSeconds(deathAudio.clip.length);
                Destroy(gameObject); // Destroys boss gameobject
            }
        }
    }
}
