using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L4BossMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator a;
    [SerializeField] private AnimationClip[] animLength;
    [SerializeField] private Transform MC;
    
    // Animation states
    private enum States { idle, walk, attack, expandFireCircleState, rocksFallState };

    public bool idle = true;
    private bool walk = false;
    private bool attack = false;
    private bool expandFireCircleAnim = false;
    private bool rocksFallAnim = false;
    private bool dead = false;

    // Prefabs
    [SerializeField] private GameObject startFireBallRainPrefab;
    GameObject startFireBallRainPrefabInstance; // used in fireBallRain() coroutine
    
    [SerializeField] private rocksFallManager rFM;
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private GameObject fireShieldPrefab;

    // Variables for my fire circle
    private int numberOfFireballs = 12;
    private float fireCircleRadius = 2.36f;
    private float rotationSpeed = 80f; // Rotation speed in degrees per second
    private float yOffset = 0.5f; // used to slightly lower the y-position of the fireballs relative to the boss' position
    
    // Declare a reference to the RotateFireballs coroutine
    private Coroutine rotateFireballsCoroutine;

    // Health
    public float currentHealth = 100f;
    public float maxHealth = 100f;

    // Audio
    [SerializeField] AudioSource swingAudio;
    [SerializeField] AudioSource maceDragAudio;
    [SerializeField] AudioSource startingDarknessAudio;
    [SerializeField] AudioSource insideDarknessAudio;
    [SerializeField] AudioSource boneShieldAudio;
    [SerializeField] AudioSource deathAudio;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        a = GetComponent<Animator>();

        StartCoroutine(follow_MC());
        StartCoroutine(fireBallRain());
        StartCoroutine(pickAbility());
        StartCoroutine(dummyBossHitTester());

    }

    // Update is called once per frame
    void Update()
    {
        // Flip boss sprite on its X axis depending on if the MC is left or right of the boss
        sr.flipX = MC.position.x > transform.position.x;

        if (idle && !attack && !expandFireCircleAnim && !rocksFallAnim)
        {
            a.SetInteger("state", (int)States.idle);
        }

        // Make the boss move towards MC when MC is NOT idle
        if (!idle && !attack && !expandFireCircleAnim && !rocksFallAnim && !dead)
        {
            transform.position = Vector2.MoveTowards(transform.position, MC.position, 5f * Time.deltaTime);
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            attack = true;
            a.SetInteger("state", (int)States.attack);
            StartCoroutine(attackFunc());
        }
    }
    // Same steps as 'OnTriggerEnter2D()'
    private void OnTriggerStay2D(Collider2D collision)
    {
        OnTriggerEnter2D(collision);
    }
    private IEnumerator attackFunc()
    {
        // swingAudio.Play();
        yield return new WaitForSeconds(animLength[0].length);
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
        // if player quickly enters and exits boss' box collider, it first triggers 'OnTriggerEnter2D()' which plays the attack animation, but I have to add this delay when exiting or else the attack animation would get interrupted by the walk/idle animation
        yield return new WaitForSeconds(animLength[0].length / 1.4f);

        attack = false;

        // If the boss was walking before the attack, transition back to walking animation
        if (walk)
        {
            a.SetInteger("state", (int)States.walk);
        }
    }

    private IEnumerator fireBallRain()
    {
        bool startFireBallRainIsActive = false;
        
        yield return new WaitForSeconds(Random.Range(5f, 10f)); // Wait 5-10 sec before giving the chance to start a fireball rain

        while (true)
        {
            if (!startFireBallRainIsActive)
            {
                startFireBallRainIsActive = true;
                GameObject startFireBallRain = Instantiate(startFireBallRainPrefab, transform.position, Quaternion.identity);
                startFireBallRain.transform.SetParent(transform);

                startFireBallRainPrefabInstance = GameObject.FindWithTag("startFireBallRain");
            }

            if (startFireBallRainPrefabInstance == null)
            {
                startFireBallRainIsActive = false;
                yield return new WaitForSeconds(Random.Range(10f, 15f));
            }

            // Since I'm not yielding a return value in any of the if-statements, if I don't put this line, the game will freeze as it would not update to the next game frame.
            yield return null;

        }
    }

    private IEnumerator expandFireCircleAnimFunction()
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
        rocksFallAnim = true;
        StartCoroutine(rocksFall());
        a.SetInteger("state", (int)States.rocksFallState);
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
        yield return new WaitForSeconds(animLength[2].length / 1.5f);
        rFM.spawnRocks();
    }

    private IEnumerator pickAbility()
    {
        yield return new WaitForSeconds(2f);

        while (true)
        {
            if(Random.Range(0f, 1f) <= 0.5f)
            {
                // Create a new list for each call to SpawnFireballs
                List<GameObject> currentFireballs = new List<GameObject>();

                // Stop the previous RotateFireballs coroutine (if any)
                if (rotateFireballsCoroutine != null)
                {
                    StopCoroutine(rotateFireballsCoroutine);
                }

                StartCoroutine(SpawnFireballs(currentFireballs));
                yield return new WaitForSeconds(15f);
            }

            else
            {
                StartCoroutine(rocksFallAnimFunction());
                yield return new WaitForSeconds(10f);
            }


        }
    }

    private IEnumerator SpawnFireballs(List<GameObject> currentFireballs)
    {
        float angleIncrement = 360f / numberOfFireballs;

        for (int i = 0; i < numberOfFireballs; i++)
        {
            float angle = i * angleIncrement;
            Vector2 spawnPosition = GetSpawnPosition(angle, fireCircleRadius);

            GameObject fireball = Instantiate(fireballPrefab, spawnPosition, Quaternion.identity);
            fireball.transform.SetParent(transform);

            currentFireballs.Add(fireball);
            
            yield return new WaitForSeconds(0.06f); // Add a slight delay between instantiations
        }

        GameObject fireShield = Instantiate(fireShieldPrefab, transform.position + new Vector3(-0.05f, -0.9f), Quaternion.identity);
        fireShield.transform.SetParent(transform);

        rotateFireballsCoroutine = StartCoroutine(RotateFireballs(fireShield, currentFireballs));
    }

    private Vector2 GetSpawnPosition(float angle, float radius)
    {
        float radians = angle * Mathf.Deg2Rad;
        float x = transform.position.x + radius * Mathf.Cos(radians);
        float y = transform.position.y + radius * Mathf.Sin(radians) - 0.5f;

        return new Vector2(x, y);
    }

    private IEnumerator RotateFireballs(GameObject fireShield, List<GameObject> currentFireballs)
    {
        float timer = 0f;
        float triggerExpansion = 8f;
        float resetTimer = -100f;

        while (true)
        {
            foreach (GameObject fireball in currentFireballs)
            {
                RotateAroundBoss(fireball);
            }

            // Check if it's time to expand shield and fire circle
            if (timer >= triggerExpansion)
            {
                Camera.main.GetComponent<ScreenShake>().Shake();

                // Call the coroutines
                
                StartCoroutine(expandShield(fireShield));
                StartCoroutine(expandFireCircle(currentFireballs));

                // Reset the timer
                timer = resetTimer;
            }

            timer += Time.deltaTime;
            yield return null;
        }

    }

    private void RotateAroundBoss(GameObject fireball)
    {
        // Need to add the y-position of the fireball prefab by the y-offset (in this case 0.5f) because I need to perform the direction and angle calculations based on this position, not on the y-direction where the y-offset (0.5f) has been subtracted.
        // Doing math operations on the subtracted y-offset gives odd results. This happens because in each iteration, I keep taking away 0.5f from the y-position and falsely calculating the distance on the 'Vector2 direction' variable, which in turn gives a false angle value, which then messes up the whole thing.
        // So I need to bring the fireball prefab's y-position back up by 0.5f and doing operations using this position
        Vector2 fireballPositionWith_yOffset = fireball.transform.position + new Vector3(0, yOffset);
        Vector2 bossPosition = transform.position;

        Vector2 direction = fireballPositionWith_yOffset - bossPosition;
        float angle = Mathf.Atan2(direction.y, direction.x) + (rotationSpeed * Mathf.Deg2Rad * Time.deltaTime);

        float newX = bossPosition.x + fireCircleRadius * Mathf.Cos(angle);
        float newY = bossPosition.y + (fireCircleRadius * Mathf.Sin(angle)) - yOffset;

        fireball.transform.position = new Vector2(newX, newY);
    }

    private IEnumerator expandShield(GameObject fireShield)
    {
        float timer = 0f;
        
        float transitionToExpand = 0.25f;

        float originalScale = 1f;
        float maxScale = 4f;

        Color currentShieldColor = fireShield.GetComponent<SpriteRenderer>().color; // 'fireShield' game object gets instantiated inside 'SpawnFireballs()' coroutine

        StartCoroutine(expandFireCircleAnimFunction());

        while (timer < transitionToExpand)
        {
            float percentageDone = timer / transitionToExpand;
            
            float newScale = Mathf.Lerp(originalScale, maxScale, percentageDone);
            currentShieldColor.a = Mathf.Lerp(1, 0, percentageDone);

            fireShield.transform.localScale = new Vector3(newScale, newScale, 1f);
            fireShield.GetComponent<SpriteRenderer>().color = currentShieldColor;
            
            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(fireShield);
    }

    private IEnumerator expandFireCircle(List<GameObject> currentFireballs)
    {
        float timer = 0f;

        float transitionToExpand = 1.5f;

        float originalRadius = fireCircleRadius;
        float maxRadius = 12f;


        while (timer < transitionToExpand)
        {
            float percentageDone = timer / transitionToExpand;

            float newRadius = Mathf.Lerp(originalRadius, maxRadius, percentageDone);
            fireCircleRadius = newRadius;

            timer += Time.deltaTime;
            yield return null;
        }

        GameObject[] fireballsToDestroy = GameObject.FindGameObjectsWithTag("fireballCircle");

        foreach (GameObject fireball in fireballsToDestroy)
        {
            Destroy(fireball);
        }

        currentFireballs.Clear();

        fireCircleRadius = originalRadius; // have to reset fireCircleRadius for the next wave of fire circle
    }

    private IEnumerator dummyBossHitTester()
    {
        while (true)
        {
            Color originalColor = sr.color;
            Color hitEffect = sr.color;

            yield return new WaitForSeconds(1f);
            currentHealth -= 1f;

            // When boss gets hit, I want to momentarily make the boss go slighlty transparent, then back to its original color
            hitEffect.a = 0.2f;
            sr.color = hitEffect;
            yield return new WaitForSeconds(0.1f);
            sr.color = originalColor;

            if (currentHealth <= 0f)
            {
                dead = true;
                GetComponent<BoxCollider2D>().enabled = false;
                rb.bodyType = RigidbodyType2D.Static;

                a.SetTrigger("death"); // show death animation
                deathAudio.Play();

                yield return new WaitForSeconds(deathAudio.clip.length - 0.5f);
                Destroy(gameObject); // Destroys boss gameobject
            }
        }
    }

    private void OnDestroy()
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
}