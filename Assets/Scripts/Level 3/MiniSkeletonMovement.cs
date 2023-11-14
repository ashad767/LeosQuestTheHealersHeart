using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniSkeletonMovement : MonoBehaviour
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

    public float health = 100f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        a = GetComponent<Animator>();
        spawnManager = MiniEnemiesSpawnManager.Instance;

        StartCoroutine(follow_MC());
        StartCoroutine(dummyBossHitTester());
    }

    // Update is called once per frame
    void Update()
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

            // want to shoot 2 arrows
            StartCoroutine(shootArrow());
            yield return new WaitForSeconds(animLength[0].length); // call to 'yield return new WaitForSeconds(animLength[0].length)' from shootArrow() coroutine returns control to follow_MC(), but I want to wait for the shooting animation to finish

            yield return new WaitForSeconds(Random.Range(0.5f, 2f)); // to have proper transition from idle to shooting another arrow

            StartCoroutine(shootArrow());
            yield return new WaitForSeconds(animLength[0].length); // call to 'yield return new WaitForSeconds(animLength[0].length)' from shootArrow() coroutine returns control to follow_MC(), but I want to wait for the shooting animation to finish

            yield return new WaitForSeconds(Random.Range(0.5f, 2f)); // to have proper transition from idle to shooting another arrow

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

    private IEnumerator dummyBossHitTester()
    {
        while (true)
        {
            Color originalColor = sr.color;
            Color hitEffect = sr.color;

            yield return new WaitForSeconds(2.5f);
            health -= Random.Range(10f, 20f);

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
                //deathAudio.Play();
                yield return new WaitForSeconds(animLength[1].length);
                Destroy(gameObject); // Destroys boss gameobject
            }
        }
    }
}
