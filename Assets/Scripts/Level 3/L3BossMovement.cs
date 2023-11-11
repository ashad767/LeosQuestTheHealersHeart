using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class L3BossMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator a;
    [SerializeField] private AnimationClip[] animLength;
    [SerializeField] private Transform MC;

    private enum States { idle, walk, attack };

    public bool idle = true;
    private bool walk = false;
    private bool attack = false;
    private bool dead = false;

    public float currentHealth = 100f;
    public float maxHealth = 100f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        a = GetComponent<Animator>();

        StartCoroutine(follow_MC());
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
            yield return new WaitForSeconds(2.5f);
            idle = false;

            walk = true;
            a.SetInteger("state", (int)States.walk);
            yield return new WaitForSeconds(5f);
            walk = false;

            idle = true;
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
                yield return new WaitForSeconds(animLength[1].length);
                Destroy(gameObject); // Destroys boss gameobject
            }
        }
    }
}
