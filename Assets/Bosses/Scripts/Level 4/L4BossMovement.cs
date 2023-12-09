using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class L4BossMovement : Entity
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator a;
    [SerializeField] private CircleCollider2D triggerCircle;
    [SerializeField] private AnimationClip[] animLength;
    [SerializeField] private Transform MC;
    [SerializeField] private Slider healthBar;

    #region  Animation states & bools
    private enum States { idle, walk, attack, expandFireCircleState, rocksFallState };

    public bool idle = true;
    private bool walk = false;

    private bool attack = false; // Used to control animation states
    private bool attackInProgress = false; // Used as a flag in case of repeated sword attacks by the boss

    private bool expandFireCircleAnim = false;
    private bool rocksFallAnim = false;
    private bool dead = false;
    #endregion

    #region Prefabs
    [SerializeField] private GameObject startFireBallRainPrefab;
    GameObject startFireBallRainPrefabInstance; // used in fireBallRain() coroutine
    #endregion

    #region Game Level Managers (Scripts)
    [SerializeField] private rocksFallManager rFM;
    [SerializeField] private fireCircleManager fCM;
    #endregion

    #region Audio
    [SerializeField] AudioSource swordClingAudio;
    [SerializeField] AudioSource roarAudio;
    [SerializeField] AudioSource deathAudio;
    #endregion

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start(); // Simply sets "CurrentHealth = maxHealth;"

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        a = GetComponent<Animator>();
        triggerCircle = GetComponentInChildren<CircleCollider2D>();

        StartCoroutine(follow_MC());
        StartCoroutine(fireBallRain());
        StartCoroutine(pickAbility());
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (MC != null)
        {
            // Flip boss sprite on its X axis depending on if the MC is left or right of the boss
            sr.flipX = MC.position.x > transform.position.x;

            if (idle && !attack && !expandFireCircleAnim && !rocksFallAnim)
            {
                a.SetInteger("state", (int)States.idle);
            }

            // Make the boss move towards MC when boss is not doing any of the following:
            if (!idle && !attack && !expandFireCircleAnim && !rocksFallAnim && !dead)
            {
                transform.position = Vector2.MoveTowards(transform.position, MC.position, 3f * Time.deltaTime);
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

            yield return new WaitForSeconds(10f);
            walk = false;

            idle = true;
            yield return new WaitForSeconds(1f);
        }
    }

    #region Attack Logic

    // Same steps as 'OnTriggerEnter2D()'
    private void OnTriggerStay2D(Collider2D collision)
    {
        OnTriggerEnter2D(collision);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!dead && collision.gameObject.CompareTag("Player"))
        {
            if (!attackInProgress)
            {
                attack = true; // Used to control animation states
                attackInProgress = true; // Used as a flag in case of repeated sword attacks by the boss
                a.SetInteger("state", (int)States.attack);

                StartCoroutine(attackFunc());
            }
        }
    }
    
    private IEnumerator attackFunc()
    {
        yield return new WaitForSeconds(animLength[0].length);
        attackInProgress = false; // Reset the attack flag to let the next attack audio & animation play (if any)
    }

    // used by event trigger in animation window
    private void playSwordSlash()
    {
        swordClingAudio.Play();
        isPlayerHit(1f);
    }

    private void isPlayerHit(float damage)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(triggerCircle.transform.position, triggerCircle.radius + 0.5f);

        foreach (Collider2D col in colliders)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                col.gameObject.GetComponent<Player>().TakeDamage(damage);
            }
        }
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
        // if player quickly enters and exits boss' box collider, it first triggers 'OnTriggerEnter2D()' which plays the attack animation, but I have to add this delay when exiting or else the attack animation would instantly get interrupted by the walk/idle animation
        yield return new WaitForSeconds(animLength[0].length / 1.45f);
        attack = false;

        // If the boss was walking before the attack, transition back to walking animation
        if (walk)
        {
            a.SetInteger("state", (int)States.walk);
        }
    }
    #endregion

    private IEnumerator fireBallRain()
    {
        bool startFireBallRainIsActive = false;
        
        yield return new WaitForSeconds(Random.Range(5f, 10f)); // Wait 5-10 sec before giving the chance to start a fireball rain

        while (MC != null && true)
        {
            if (!startFireBallRainIsActive)
            {
                startFireBallRainIsActive = true;
                GameObject startFireBallRain = Instantiate(startFireBallRainPrefab, transform.position, Quaternion.identity);
                startFireBallRain.transform.SetParent(transform);
                startFireBallRain.GetComponent<FireBallRain>().isBossDead = this;

                startFireBallRainPrefabInstance = GameObject.FindWithTag("startFireBallRain");
            }

            // The 'Start Fireball Rain Prefab' is the fire that grows/shrinks from the boss' head, which in turn shoots fireballs
            // The 'Start Fireball Rain Prefab Instance' gets destroyed in 'FireBallRain.cs' script, which only then can this if-statement run
            if (startFireBallRainPrefabInstance == null)
            {
                startFireBallRainIsActive = false;
                yield return new WaitForSeconds(Random.Range(10f, 15f));
            }

            // Since I'm not yielding a return value in any of the if-statements, if I don't put this line, the game will freeze as it would not update to the next game frame.
            yield return null;
        }
    }

    // Called in 'fireCircleManager.cs' script
    public IEnumerator expandFireCircleAnimFunction()
    {
        expandFireCircleAnim = true;
        a.SetInteger("state", (int)States.expandFireCircleState);
        yield return new WaitForSeconds(animLength[1].length);
        expandFireCircleAnim = false;

        // If the boss was walking before this animation, transition back to walking animation
        if (walk)
        {
            a.SetInteger("state", (int)States.walk);
        }
    }

    private IEnumerator rocksFallAnimFunction()
    {
        StartCoroutine(rocksFall());
        
        rocksFallAnim = true;
        a.SetInteger("state", (int)States.rocksFallState);
        roarAudio.Play();
        yield return new WaitForSeconds(animLength[2].length);
        rocksFallAnim = false;

        // If the boss was walking before this animation, transition back to walking animation
        if (walk)
        {
            a.SetInteger("state", (int)States.walk);
        }
    }

    private IEnumerator rocksFall()
    {
        // Want to start spawning the rocks a bit after halfway through the animation
        yield return new WaitForSeconds(animLength[2].length / 1.5f);
        rFM.spawnRocks();
    }

    private IEnumerator pickAbility()
    {
        yield return new WaitForSeconds(2f);

        while (MC != null && true)
        {
            if(Random.Range(0f, 1f) <= 0.5f)
            {
                fCM.spawnFireCircle();
                yield return new WaitForSeconds(17f);
            }

            else
            {
                StartCoroutine(rocksFallAnimFunction());
                yield return new WaitForSeconds(10f);
            }

        }
    }

    //private IEnumerator TakeDamageAnimation()
    //{
    //    Color originalColor = sr.color;
    //    Color hitEffect = sr.color;

    //    // When boss gets hit, I want to momentarily make the boss go slighlty transparent, then back to its original color
    //    hitEffect.a = 0.2f;
    //    sr.color = hitEffect;
    //    yield return new WaitForSeconds(0.1f);
    //    sr.color = originalColor;
    //}


    private IEnumerator BossDeath()
    {
        dead = true;
        rb.bodyType = RigidbodyType2D.Static;
        a.SetTrigger("death"); // show death animation
        deathAudio.Play();

        destroyGameManagers();
        destroyPrefabs();

        yield return new WaitForSeconds(deathAudio.clip.length - 0.5f);
        Destroy(gameObject); // Destroys boss gameobject
    }

    private void destroyGameManagers()
    {
        GameObject[] gameManagersToDestroy = GameObject.FindGameObjectsWithTag("GameManagers");

        foreach (GameObject gm in gameManagersToDestroy)
        {
            Destroy(gm);
        }
    }

    private void destroyPrefabs()
    {
        // Find all active prefabs in the scene and destroy them
        GameObject[] fireballCircleToDestroy = GameObject.FindGameObjectsWithTag("fireballCircle");
        GameObject[] fireballToDestroy = GameObject.FindGameObjectsWithTag("fireball");
        GameObject[] rocksToDestroy = GameObject.FindGameObjectsWithTag("rocks");


        foreach (GameObject fireballCircle in fireballCircleToDestroy)
        {
            Destroy(fireballCircle);
        }

        foreach (GameObject fireball in fireballToDestroy)
        {
            Destroy(fireball);
        }

        foreach (GameObject rock in rocksToDestroy)
        {
            Destroy(rock);
        }
    }


    private void OnDestroy()
    {
        Destroy(healthBar.gameObject);
    }
}