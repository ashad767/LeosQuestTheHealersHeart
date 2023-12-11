using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireCircleManager : MonoBehaviour
{
    [SerializeField] private Transform Boss; // to get the boss' position
    [SerializeField] private L4BossMovement bossScript; // to call the 'expandFireCircleAnimFunction()' coroutine

    // Prefabs
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private GameObject fireShieldPrefab;

    // Variables for my fire circle
    private int numberOfFireballs = 13;
    private float rotationSpeed = 80f; // Rotation speed in degrees per second
    
    private float fireCircleRadius = 2.47f;
    private float xOffset = 0.12f;
    private float yOffset = 1.15f; // Used to slightly lower the y-position of the fireballs relative to the boss' position
    
    private bool isExploding = false;

    // Declare a reference to the RotateFireballsManager() coroutine
    private Coroutine rotateFireballsManagerCoroutine;

    // Audio
    [SerializeField] AudioSource randomScreamAudio;
    [SerializeField] AudioSource spawnFireballAudio;
    [SerializeField] AudioSource fireShieldActivatedAudio;
    [SerializeField] AudioSource fireShieldNoiseAudio;
    [SerializeField] AudioSource fireCircleAboutToExplodeAudio;
    [SerializeField] AudioSource fireCircleExplosionAudio;

    
    public void spawnFireCircle()
    {
        // Create a new list for each call to SpawnFireballs
        List<GameObject> currentFireballs = new List<GameObject>();

        StartCoroutine(SpawnFireballs(currentFireballs));
    }

    private IEnumerator SpawnFireballs(List<GameObject> currentFireballs)
    {
        float angleIncrement = 360f / numberOfFireballs;

        for (int i = 0; i < numberOfFireballs; i++)
        {
            float angle = i * angleIncrement;
            Vector2 spawnPosition = GetSpawnPosition(angle, fireCircleRadius);

            GameObject fireball = Instantiate(fireballPrefab, spawnPosition, Quaternion.identity);
            fireball.transform.SetParent(Boss);
            fireball.GetComponent<BoxCollider2D>().enabled = false;
            spawnFireballAudio.Play();

            currentFireballs.Add(fireball);

            yield return new WaitForSeconds(0.06f); // Add a slight delay between instantiations
        }

        GameObject fireShield = Instantiate(fireShieldPrefab, Boss.position + new Vector3(0.1f, -1.36f, 0), Quaternion.identity);
        fireShield.transform.SetParent(Boss);
        fireShieldActivatedAudio.Play();
        fireShieldNoiseAudio.Play();

        rotateFireballsManagerCoroutine = StartCoroutine(RotateFireballsManager(fireShield, currentFireballs));
    }

    private Vector2 GetSpawnPosition(float angle, float radius)
    {
        float radians = angle * Mathf.Deg2Rad;
        float x = Boss.position.x + radius * Mathf.Cos(radians) + xOffset;
        float y = Boss.position.y + radius * Mathf.Sin(radians) - yOffset;

        return new Vector2(x, y);
    }

    private IEnumerator RotateFireballsManager(GameObject fireShield, List<GameObject> currentFireballs)
    {
        float timer = 0f;
        float triggerExpansion = Random.Range(8f, 12f);
        float resetTimer = -100f;

        float timeToWaitBeforeStartingfireCircleAboutToExplodeAudio = triggerExpansion - fireCircleAboutToExplodeAudio.clip.length;
        StartCoroutine(playFireCircleAboutToExplodeAudio(timeToWaitBeforeStartingfireCircleAboutToExplodeAudio));

        while (true)
        {
            foreach (GameObject fireball in currentFireballs)
            {
                if(fireball != null)
                {
                    RotateAroundBoss(fireball);
                }
            }

            // Check if it's time to expand shield and fire circle
            if (timer >= triggerExpansion)
            {
                //Camera.main.GetComponent<ScreenShake>().Shake();
                isExploding = true;

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
        // Need to add the y-position of the fireball prefab by the y-offset (in this case 0.5f) because I need to perform the direction and angle calculations based on this position, not on the y-direction where the y-offset has been subtracted.
        // Doing math operations on the subtracted y-offset gives odd results. This happens because in each iteration, I keep taking away 0.5f from the y-position and falsely calculating the distance on the 'Vector2 direction' variable, which in turn gives a false angle value, which then messes up the whole thing.
        // So I need to bring the fireball prefab's y-position back up by yOffset and doing operations using this position
        Vector2 fireballPositionWith_yOffset = fireball.transform.position + new Vector3(-xOffset, yOffset);
        Vector2 bossPosition = Boss.position;
        rotationSpeed = isExploding ? 135f : 80f;

        Vector2 direction = fireballPositionWith_yOffset - bossPosition;
        float angle = Mathf.Atan2(direction.y, direction.x) + (rotationSpeed * Mathf.Deg2Rad * Time.deltaTime);

        float newX = bossPosition.x + fireCircleRadius * Mathf.Cos(angle) + xOffset;
        float newY = bossPosition.y + (fireCircleRadius * Mathf.Sin(angle)) - yOffset;

        fireball.transform.position = new Vector2(newX, newY);
    }


    private IEnumerator playFireCircleAboutToExplodeAudio(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        fireCircleAboutToExplodeAudio.Play();
    }

    private IEnumerator expandShield(GameObject fireShield)
    {
        float timer = 0f;

        float transitionToExpand = 0.25f;

        float originalScale = 1f;
        float maxScale = 4f;

        float origDamage = 5f;
        fireShieldCheckCollision fscc = fireShield.GetComponent<fireShieldCheckCollision>();

        Color currentShieldColor = fireShield.GetComponent<SpriteRenderer>().color; // 'fireShield' game object gets instantiated inside 'SpawnFireballs()' coroutine

        // Play/Stop audios
        randomScreamAudio.Play();
        fireShieldNoiseAudio.Stop();
        fireCircleExplosionAudio.Play();

        // Trigger the animation
        StartCoroutine(bossScript.expandFireCircleAnimFunction());

        while (timer < transitionToExpand)
        {
            float percentageDone = timer / transitionToExpand;

            float newScale = Mathf.Lerp(originalScale, maxScale, percentageDone);
            float newDamage = Mathf.Lerp(origDamage, 0f, percentageDone);
            currentShieldColor.a = Mathf.Lerp(1, 0, percentageDone);

            fireShield.transform.localScale = new Vector3(newScale, newScale, 1f);
            fscc.damage = Mathf.Round(newDamage);
            fireShield.GetComponent<SpriteRenderer>().color = currentShieldColor;

            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(fireShield);
    }

    private IEnumerator expandFireCircle(List<GameObject> currentFireballs)
    {
        float timer = 0f;

        float transitionToExpand = 1f;

        float originalRadius = fireCircleRadius;
        float maxRadius = 12f;

        foreach (GameObject fireball in currentFireballs)
        {
            fireball.GetComponent<BoxCollider2D>().enabled = true;
        }

        
        while (timer < transitionToExpand)
        {
            float percentageDone = timer / transitionToExpand;

            float newRadius = Mathf.Lerp(originalRadius, maxRadius, percentageDone);
            fireCircleRadius = newRadius;

            timer += Time.deltaTime;
            yield return null;
        }

        
        foreach (GameObject fireball in currentFireballs)
        {
            Destroy(fireball);
        }

        StopCoroutine(rotateFireballsManagerCoroutine);
        currentFireballs.Clear();

        fireCircleRadius = originalRadius; // Have to reset fireCircleRadius for the next fire circle (if called)
        isExploding = false;
    }
}