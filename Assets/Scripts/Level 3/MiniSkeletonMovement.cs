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
            transform.position = Vector2.MoveTowards(transform.position, MC.position, Random.Range(1f, 2f) * Time.deltaTime);
        }
    }

    private IEnumerator follow_MC()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            idle = false;

            a.SetInteger("state", (int)States.walk);
            yield return new WaitForSeconds(2f);

            idle = true;
            yield return new WaitForSeconds(0.5f); // to have proper transition from idle to shooting arrow

            // want to shoot 2 arrows
            StartCoroutine(shootArrow());
            yield return new WaitForSeconds(animLength[0].length); // call to 'yield return new WaitForSeconds(animLength[0].length)' from shootArrow() coroutine returns control to follow_MC(), but I want to wait for the shooting animation to finish

            yield return new WaitForSeconds(0.5f); // to have proper transition from idle to shooting arrow

            StartCoroutine(shootArrow());
            yield return new WaitForSeconds(animLength[0].length); // call to 'yield return new WaitForSeconds(animLength[0].length)' from shootArrow() coroutine returns control to follow_MC(), but I want to wait for the shooting animation to finish
        }
    }

    private IEnumerator shootArrow()
    {
        shoot = true;
        a.SetInteger("state", (int)States.shoot);
        yield return new WaitForSeconds(animLength[0].length);

        GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
        arrow.GetComponent<Arrow>().arrowDir = MC.position - arrow.transform.position;

        shoot = false;
    }

    private IEnumerator dummyBossHitTester()
    {
        while (true)
        {
            Color originalColor = sr.color;
            Color hitEffect = sr.color;

            yield return new WaitForSeconds(2.5f);
            health -= 40f;

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

                a.SetTrigger("death"); // show death animation
                //deathAudio.Play();
                yield return new WaitForSeconds(animLength[1].length);
                Destroy(gameObject); // Destroys boss gameobject
            }
        }
    }
}
