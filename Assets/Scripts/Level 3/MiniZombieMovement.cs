using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniZombieMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator a;
    [SerializeField] private AnimationClip[] animLength;
    
    public Transform MC;

    private MiniEnemiesSpawnManager spawnManager;
    public darknessManager darknessManager; // the script

    // Audio
    [SerializeField] AudioSource zombieSpawnAudio;
    [SerializeField] AudioSource zombieMoveAudio;
    [SerializeField] AudioSource deathAudio;

    private enum States { walk, attack };

    private bool isWalkingSlow = true;
    private float currentSpeed;
    private float walkTimer;

    private bool attack = false; // Used to control animation states
    private bool attackInProgress = false; // Used as a flag in case of repeated sword attacks by the mini-zombie
    private bool dead = false;

    public float health = 100f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        a = GetComponent<Animator>();
        spawnManager = MiniEnemiesSpawnManager.Instance;

        // Initialize timers
        walkTimer = Random.Range(3f, 6f); // Adjust the initial walk duration
        SetNewSpeed();

        StartCoroutine(dummyBossHitTester());
        zombieSpawnAudio.Play();
        Invoke("PlayZombieMoveAudio", zombieSpawnAudio.clip.length + Random.Range(0.2f, 1.2f));
    }

    // Update is called once per frame
    void Update()
    {
        // used for my Blend Tree
        Vector2 dir = MC.position - transform.position;
        dir.Normalize();
        a.SetFloat("dirX", dir.x);
        a.SetFloat("dirY", dir.y);

        if(!attack && !dead)
        {
            // Update timer
            walkTimer -= Time.deltaTime;

            // Check if the walk duration is over
            if (walkTimer <= 0f)
            {
                // Reset the timer
                walkTimer = Random.Range(2f, 7f);
                SetNewSpeed();
            }

            transform.position = Vector2.MoveTowards(transform.position, MC.position, currentSpeed * Time.deltaTime);
        }
    }

    private void PlayZombieMoveAudio()
    {
        zombieMoveAudio.Play();
    }

    // Function to set a new speed based on the current pace
    private void SetNewSpeed()
    {
        if (isWalkingSlow)
        {
            currentSpeed = Random.Range(1.5f, 2.5f);
        }
        else
        {
            currentSpeed = Random.Range(4.5f, 6.5f);
        }

        isWalkingSlow = !isWalkingSlow; // Switch the pace for next call
    }

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
                attack = true;
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

        a.SetInteger("state", (int)States.walk);

    }

    
    private IEnumerator dummyBossHitTester()
    {
        while (true)
        {
            Color originalColor = sr.color;
            Color hitEffect = sr.color;

            yield return new WaitForSeconds(2.5f);
            health -= Random.Range(5f, 20f);

            // When boss gets hit, I want to momentarily make the boss go slighlty transparent, then back to its original/angry color
            hitEffect.a = 0.2f;
            sr.color = hitEffect;
            yield return new WaitForSeconds(0.1f);
            sr.color = originalColor;

            if (health <= 0f)
            {
                dead = true;
                GetComponent<BoxCollider2D>().enabled = false;
                rb.bodyType = RigidbodyType2D.Static;
                spawnManager.MiniEnemyKilled();
                darknessManager.spawnedMiniEnemies.Remove(gameObject);

                a.SetTrigger("death"); // show death animation
                zombieMoveAudio.Stop();
                deathAudio.Play();
                yield return new WaitForSeconds(animLength[1].length);
                Destroy(gameObject); // Destroys boss gameobject
            }
        }
    }
}
