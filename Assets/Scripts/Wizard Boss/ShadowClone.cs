using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;

public class ShadowClone : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator a;
    [SerializeField] private AnimationClip[] shadowAnimLength;

    private float bossMoveSpeed;
    private float health = 50f;
    private Color originalColor;

    // Getting these values from Wizard.cs when I instantiate a shadow clone.
    // Took me a long time to figure out that you can't just drag in and drop gameObjects from hierarchy window into prefab scripts in Inspector window.
    // Have to get a reference/s from the script that instantiates this prefab and assign values from there to use it here.
    public bool isBossAngry;
    public GameObject MC;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        a = GetComponent<Animator>();

        bossMoveSpeed = isBossAngry ? Random.Range(4.5f, 6f) : Random.Range(3.7f, 4.5f);
        originalColor = sr.color;

        StartCoroutine(lungeAnim());
        StartCoroutine(dummyShadowHitTester());
    }

    // Update is called once per frame
    void Update()
    {
        // Flip boss sprite on its X axis depending on if the MC is left or right of the boss
        sr.flipX = MC.transform.position.x > transform.position.x;

        transform.position = Vector2.MoveTowards(transform.position, MC.transform.position, bossMoveSpeed * Time.deltaTime);
    }

    private IEnumerator lungeAnim()
    {
        // I want the boss to lunge at the MC based on the MC's distance and direction.
        // So I first take the direction in which the boss has to lunge towards (-1 or 1, in X or Y direction)
        // Then multiply by the distance between the boss and MC. But that would be too fast,so I divided the distance by either 1.5 or 2 to make it slower
        // I then put those X & Y values as the velocity for the boss 
        float whereIsMC_X = Mathf.Sign(MC.transform.position.x);
        float whereIsMC_Y = Mathf.Sign(MC.transform.position.y);
        float lungeSpeedX = Mathf.Abs(MC.transform.position.x - transform.position.x);
        float lungeSpeedY = Mathf.Abs(MC.transform.position.y - transform.position.y);

        float lungeSpeedController = isBossAngry ? Random.Range(1.2f, 1.4f) : Random.Range(1.4f, 1.8f);
        rb.velocity = new Vector2(whereIsMC_X * (lungeSpeedX / lungeSpeedController), whereIsMC_Y * (lungeSpeedY / lungeSpeedController));

        yield return new WaitForSeconds(shadowAnimLength[0].length + 0.6f);

        a.SetBool("idle", true);

        rb.velocity = new Vector2(0, 0); // stops boss from drifting away after lunging
    }

    private IEnumerator dummyShadowHitTester()
    {
        while (true)
        {
            Color hitEffect = originalColor;

            yield return new WaitForSeconds(2.5f);
            health -= 20f;

            // When boss gets hit, I want to momentarily make the boss go slighlty transparent, then back to its original color
            hitEffect.a = 0.3f;
            sr.color = hitEffect;
            yield return new WaitForSeconds(0.12f);
            sr.color = originalColor;

            if (health <= 0f)
            {
                a.SetTrigger("shadowDeath"); // show death animation
                yield return new WaitForSeconds(shadowAnimLength[1].length);
                Destroy(gameObject);
            }
        }
    }
}
