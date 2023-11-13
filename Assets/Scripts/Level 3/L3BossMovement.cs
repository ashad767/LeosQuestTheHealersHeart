using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class L3BossMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator a;
    [SerializeField] private AnimationClip[] animLength;
    [SerializeField] private Transform MC;

    // Prefabs
    public GameObject lightningPrefab;
    public GameObject miniZombiePrefab;
    public GameObject miniSkeletonPrefab;
    [SerializeField] private GameObject startingDarknessPrefab;

    private MiniEnemiesSpawnManager spawnManager;
    [SerializeField] private darknessManager darknessManager; // the script
    [SerializeField] private GameObject directionalLight;
    [SerializeField] private GameObject MC_PointLight;

    // Animation states
    private enum States { idle, walk, attack };

    public bool idle = true;
    private bool walk = false;
    private bool attack = false;
    private bool dead = false;
    
    public float currentHealth = 100f;
    public float maxHealth = 100f;

    private bool isDark = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        a = GetComponent<Animator>();
        
        spawnManager = MiniEnemiesSpawnManager.Instance;
        directionalLight.SetActive(false);
        MC_PointLight.SetActive(false);

        StartCoroutine(follow_MC());
        StartCoroutine(Darken());
        StartCoroutine(dummyBossHitTester());
    }

    // Update is called once per frame
    void Update()
    {
        // Flip boss sprite on its X axis depending on if the MC is left or right of the boss
        sr.flipX = MC.position.x < transform.position.x;

        if (idle && !attack)
        {
            a.SetInteger("state", (int)States.idle);
        }

        // Make the boss move towards MC when MC is NOT idle
        if (!idle && !dead)
        {
            transform.position = Vector2.MoveTowards(transform.position, MC.position, 2.7f * Time.deltaTime);
        }
    }

    private IEnumerator follow_MC()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            idle = false;

            walk = true;
            a.SetInteger("state", (int)States.walk);
            yield return new WaitForSeconds(1.5f);
            walk = false;

            idle = true;
            
            if(Random.Range(0f, 1f) <= 0.99f)
            {
                spawnManager.StartSpawning();
            }

            yield return new WaitForSeconds(4f);
        }
    }
    private IEnumerator Darken()
    {
        while (true)
        {
            // Because I don't want the chance of darkening the screen at the start of the game, I check if 0.1 seconds have passed since the beginning of the game, then wait 8 sec before starting the chance to darken the screen.
            if(Time.time <= 0.1f)
            {
                yield return new WaitForSeconds(8f);
            }
            
            // 85% chance of the screen getting dark
            if (Random.Range(0f, 1f) <= 0.85f)
            {
                GameObject startingDarkness = Instantiate(startingDarknessPrefab, transform.position, Quaternion.identity);
                startingDarkness.transform.SetParent(transform); // if the boss moves, the animation moves with it
                Destroy(startingDarkness, animLength[3].length * 2); // playing the animation twice before destroying it

                darknessManager.activateDarkness();
                isDark = true;
            }

            // if the darkness effect gets activated, I want to wait 10 seconds before deactivating it. And since I don't want to darken the screen right away again, I wait another 7 sec.
            if (isDark)
            {
                yield return new WaitForSeconds(10f);
                darknessManager.de_activateDarkness();
                isDark = false;

                yield return new WaitForSeconds(7f);
            }
            
            // if the darkness effect does NOT get activated, just wait 2 sec before giving the chance to darken the screen.
            else
            {
                yield return new WaitForSeconds(2f);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            attack = true;
            a.SetInteger("state", (int)States.attack);
            //swordSwing.Play();
            StartCoroutine(attackFunc());
        }
    }
    private IEnumerator attackFunc()
    {
        yield return new WaitForSeconds(animLength[0].length);
        attack = false;

        // because walking is a looped animation, if boss attacks midway of the walking animation, I want to return back to the walking animation
        if (walk)
        {
            a.SetInteger("state", (int)States.walk);
        }
    }

    private IEnumerator dummyBossHitTester()
    {
        while (true)
        {
            Color originalColor = sr.color;
            Color hitEffect = sr.color;

            yield return new WaitForSeconds(2.5f);
            currentHealth -= 5f;

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
                yield return new WaitForSeconds(animLength[1].length);
                Destroy(gameObject); // Destroys boss gameobject
            }
        }
    }

}
