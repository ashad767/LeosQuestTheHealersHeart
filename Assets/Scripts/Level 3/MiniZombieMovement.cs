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

    private enum States { walk, attack };

    public float health = 100f;
    private bool dead = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        a = GetComponent<Animator>();
        spawnManager = MiniEnemiesSpawnManager.Instance;

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

        if(!dead)
        {
            transform.position = Vector2.MoveTowards(transform.position, MC.position, Random.Range(2f, 3.5f) * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            a.SetInteger("state", (int)States.attack);
            //swordSwing.Play();
            StartCoroutine(attackFunc());
        }
    }

    private IEnumerator attackFunc()
    {
        yield return new WaitForSeconds(animLength[0].length);

        a.SetInteger("state", (int)States.walk);
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
