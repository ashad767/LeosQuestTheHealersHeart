using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniSkeletonMovement : Entity
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator a;
    [SerializeField] private AnimationClip[] animLength;
    public Transform MC;

    [SerializeField] private GameObject arrowPrefab;

    private MiniEnemiesSpawnManager spawnManager;
    public darknessManager darknessManager; // the script

    // Audio
    [SerializeField] AudioSource shootArrowAudio;
    [SerializeField] AudioSource movementAudio;

    private enum States { idle, walk, shoot };

    public bool idle = true;
    private bool shoot = false;
    private bool dead = false;
    private bool collidingWithplayer = false;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start(); // Simply sets "CurrentHealth = maxHealth;"

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        a = GetComponent<Animator>();
        spawnManager = MiniEnemiesSpawnManager.Instance;

        StartCoroutine(follow_MC());
    }

    // Update is called once per frame
    protected override void Update()
    {
        if(MC != null)
        {
            // used for my Blend Tree
            Vector2 dir = MC.position - transform.position;
            dir.Normalize();
            a.SetFloat("dirX", dir.x);
            a.SetFloat("dirY", dir.y);

            if (idle && !shoot)
            {
                a.SetInteger("state", (int)States.idle);
            }

            if (!idle && !dead)
            {
                transform.position = Vector2.MoveTowards(transform.position, MC.position, Random.Range(0.7f, 3f) * Time.deltaTime);
            }
        }

        else
        {
            // death when player dies
            if (!dead)
            {
                StartCoroutine(miniSkeletonDeath());
            }
        }

        // death
        if (GetHealth() <= 0 && !dead)
        {
            StartCoroutine(miniSkeletonDeath());
        }
    }

    private IEnumerator follow_MC()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            idle = false;

            a.SetInteger("state", (int)States.walk);
            movementAudio.Play();
            yield return new WaitForSeconds(2f);
            movementAudio.Stop();

            idle = true;
            yield return new WaitForSeconds(0.5f); // to have proper transition from idle to shooting arrow

            // want to shoot 2 arrows when becoming idle
            StartCoroutine(shootArrow());
            yield return new WaitForSeconds(animLength[0].length); // call to 'yield return new WaitForSeconds(animLength[0].length)' from shootArrow() coroutine returns control to follow_MC(), but I want to wait for the shooting animation to finish

            yield return new WaitForSeconds(Random.Range(0.5f, 2f)); // wait before shooting another arrow

            StartCoroutine(shootArrow());
            yield return new WaitForSeconds(animLength[0].length); // call to 'yield return new WaitForSeconds(animLength[0].length)' from shootArrow() coroutine returns control to follow_MC(), but I want to wait for the shooting animation to finish

            yield return new WaitForSeconds(Random.Range(0.5f, 2f)); // wait before shooting another arrow

            // 60% chance of shooting a 3rd arrow
            if (Random.Range(0f, 1f) <= 0.6f)
            {
                StartCoroutine(shootArrow());
                yield return new WaitForSeconds(animLength[0].length);
            }
        }
    }

    private IEnumerator shootArrow()
    {
        shoot = true;
        a.SetInteger("state", (int)States.shoot);
        yield return new WaitForSeconds(animLength[0].length);

        GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
        arrow.GetComponent<Arrow>().arrowDir = MC.position - arrow.transform.position;
        shootArrowAudio.Play();

        shoot = false;
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
    #endregion

    private IEnumerator miniSkeletonDeath()
    {
        dead = true;
        rb.bodyType = RigidbodyType2D.Static;

        spawnManager.MiniEnemyKilled();
        darknessManager.spawnedMiniEnemies.Remove(gameObject);

        a.SetTrigger("death"); // show death animation

        yield return new WaitForSeconds(animLength[1].length);
        Destroy(gameObject); // Destroys boss gameobject
    }
}
