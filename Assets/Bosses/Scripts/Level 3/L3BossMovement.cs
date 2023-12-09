using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.GlobalIllumination;

public class L3BossMovement : Entity
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator a;
    [SerializeField] private CircleCollider2D triggerCircle;
    [SerializeField] private AnimationClip[] animLength;
    [SerializeField] private Transform MC;
    [SerializeField] private Slider healthBar;

    #region Prefabs
    public GameObject lightningPrefab;
    public GameObject miniZombiePrefab;
    public GameObject miniSkeletonPrefab;
    [SerializeField] private GameObject startingDarknessPrefab;
    [SerializeField] private GameObject boneShieldPrefab;
    #endregion

    #region Game Level Managers (Scripts)
    private MiniEnemiesSpawnManager spawnManager; // Singleton
    [SerializeField] private darknessManager darknessManager;
    #endregion

    #region Tools for Darkness Effect
    [SerializeField] private GameObject directionalLight;
    [SerializeField] private GameObject MC_PointLight;
    #endregion

    #region Audio
    [SerializeField] AudioSource swingAudio;
    [SerializeField] AudioSource maceDragAudio;
    [SerializeField] AudioSource startingDarknessAudio;
    [SerializeField] AudioSource insideDarknessAudio;
    [SerializeField] AudioSource boneShieldAudio;
    [SerializeField] AudioSource deathAudio;
    #endregion
    
    #region Animation states & bools
    private enum States { idle, walk, attack };

    public bool idle = true;
    private bool walk = false;

    private bool attack = false; // Used to control animation states
    private bool attackInProgress = false; // Used as a flag in case of repeated sword attacks by the boss
    
    private bool dead = false;

    private bool boneShieldActive = false;
    private float snapshotHealth;
    #endregion


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start(); // Simply sets "CurrentHealth = maxHealth;"

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        a = GetComponent<Animator>();
        
        spawnManager = MiniEnemiesSpawnManager.Instance;
        directionalLight.SetActive(false);
        MC_PointLight.SetActive(false);

        StartCoroutine(follow_MC());
        StartCoroutine(spawnMiniEnemies());
        StartCoroutine(Darken());
    }

    // Update is called once per frame
    protected override void Update()
    {
        if(MC != null)
        {
            if(boneShieldActive)
            {
                CurrentHealth = snapshotHealth;
            }
            else
            {
                snapshotHealth = GetHealth();
            }

            // Flip boss sprite on its X axis depending on if the MC is left or right of the boss
            sr.flipX = MC.position.x < transform.position.x;

            if (idle && !attack)
            {
                a.SetInteger("state", (int)States.idle);
            }

            // Make the boss move towards MC when MC is NOT idle
            if (!idle && !attack && !dead)
            {
                transform.position = Vector2.MoveTowards(transform.position, MC.position, 2.7f * Time.deltaTime);
            }
        }

        // If player dies, just go idle
        else
        {
            a.SetInteger("state", (int)States.idle);
            StopAllCoroutines();
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
                maceDragAudio.Play();
            }
            yield return new WaitForSeconds(3f);
            maceDragAudio.Stop();
            walk = false;

            idle = true;
            yield return new WaitForSeconds(4f);
        }
    }

    private IEnumerator spawnMiniEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(5f, 10f)); // Wait 5-10 seconds before getting the chance to spawn mini-enemies

            // 95% chance of mini-enemies getting spawned (if not the first iteration, only spawns if all 3 mini-enemies are killed from previous wave)
            if (Random.Range(0f, 1f) <= 0.95f)
            {
                spawnManager.StartSpawning();
            }
        }
    }

    private IEnumerator Darken()
    {
        float chanceOfDarkness = 0.65f;
        yield return new WaitForSeconds(8f); // Wait 8 sec before giving the chance to darken screen

        while (true)
        {
            // 65% chance of the screen getting dark on every other iteration (starts from 65%, then if dark, stay at 65%. If shield gets activated instead, chance of darkness is 25% on next iteration)
            if (!dead && (Random.Range(0f, 1f) <= chanceOfDarkness))
            {
                startingDarknessAudio.Play();
                GameObject startingDarkness = Instantiate(startingDarknessPrefab, transform.position, Quaternion.identity);
                startingDarkness.transform.SetParent(transform); // if the boss moves, the animation moves with it
                Destroy(startingDarkness, animLength[3].length * 2); // playing the animation twice before destroying it

                darknessManager.activateDarkness();
            }

            // if the darkness effect gets activated, I want to wait 35 seconds before deactivating it. And since I don't want to darken the screen right away again, I wait another 5 sec.
            if (darknessManager.isDark)
            {
                StartCoroutine(PlayInsideDarknessAudio());
                yield return new WaitForSeconds(35f);
                darknessManager.de_activateDarkness(5f);

                chanceOfDarkness = 0.25f;
                yield return new WaitForSeconds(5f);
            }

            // if the darkness effect does NOT get activated, then activate bone shield and set the chanceOfDarkness to 65%
            else
            {
                StartCoroutine(activateBoneShield());
                yield return new WaitForSeconds(8f); // control comes back here after first yield call in 'activateBoneShield()'. Bone shield effect lasts 8 sec, so I wait 8 seconds here as well to let the bone shield effect finish properly.

                chanceOfDarkness = 0.65f;
                yield return new WaitForSeconds(2f); // Wait another 2 seconds just for the sake of it
            }
        }
    }

    private IEnumerator PlayInsideDarknessAudio()
    {
        yield return new WaitForSeconds(startingDarknessAudio.clip.length - 5.5f);
        insideDarknessAudio.Play();
    }

    private IEnumerator activateBoneShield()
    {
        boneShieldActive = true;
        boneShieldAudio.Play();
        GameObject boneShield = Instantiate(boneShieldPrefab, transform.position, Quaternion.identity);
        boneShield.transform.SetParent(transform);

        yield return new WaitForSeconds(8f); // let the bone shield effect last 8 seconds
        
        boneShieldActive = false;
        Destroy(boneShield);
    }

    #region Attack Logic

    // Same steps as 'OnTriggerEnter2D()'
    private void OnTriggerStay2D(Collider2D collision)
    {
        OnTriggerEnter2D(collision);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!attackInProgress)
            {
                attack = true; // Used to control animation states
                attackInProgress = true; // Used as a flag in case of repeated sword attacks by the boss
                a.SetInteger("state", (int)States.attack);
                maceDragAudio.Stop();

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
    private void playMaceSwing()
    {
        swingAudio.Play();
        isPlayerHit();
    }

    private void isPlayerHit()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(triggerCircle.transform.position, triggerCircle.radius + 0.5f);

        foreach (Collider2D col in colliders)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                col.gameObject.GetComponent<Player>().TakeDamage(1f);
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
        yield return new WaitForSeconds(animLength[0].length / 1.5f);
        attack = false;

        // If the boss was walking before the attack, transition back to walking animation
        if (walk)
        {
            a.SetInteger("state", (int)States.walk);
            maceDragAudio.Play();
        }
    }
    #endregion


    private IEnumerator BossDeath()
    {
        dead = true;
        rb.bodyType = RigidbodyType2D.Static;
        a.SetTrigger("death"); // show death animation
        deathAudio.Play();

        destroyChildren();

        yield return new WaitForSeconds(deathAudio.clip.length);

        if (darknessManager.isDark) { darknessManager.de_activateDarkness(2f); }
        destroyGameManagers();

        Destroy(gameObject); // Destroys boss gameobject
    }

    private void destroyChildren()
    {
        // Iterate through each child of the boss GameObject
        foreach (Transform child in transform)
        {
            // Destroy the child GameObject
            Destroy(child.gameObject);
        }
    }

    private void destroyGameManagers()
    {
        GameObject gm = GameObject.FindGameObjectWithTag("GameManagers");
        Destroy(gm, 2.2f); // Wait for the to the light 2 come back (if any, from darkness manager), then destroy all game managers
    }

    private void OnDestroy()
    {
        healthBar.gameObject.SetActive(false); // Hide the boss healthbar from view after boss dies

        // Find all active mini-zombies/skeleton and arrow prefabs in the scene and destroy them
        GameObject[] miniEnemiesToDestroy = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] arrowsToDestroy = GameObject.FindGameObjectsWithTag("arrow");

        foreach (GameObject miniEnemy in miniEnemiesToDestroy)
        {
            Destroy(miniEnemy);
        }

        foreach (GameObject arrow in arrowsToDestroy)
        {
            Destroy(arrow);
        }
    }
}