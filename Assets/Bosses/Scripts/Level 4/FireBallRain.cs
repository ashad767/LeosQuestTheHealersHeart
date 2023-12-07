using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class FireBallRain : MonoBehaviour
{
    [SerializeField] private GameObject fireBallPrefab;

    // Audio
    [SerializeField] AudioSource fireballRainStartAudio;
    [SerializeField] AudioSource fireballRainMiddleAudio;
    [SerializeField] AudioSource fireballRainEndAudio;
    [SerializeField] AudioSource shootFireballAudio;

    // Start is called before the first frame update
    void Start()
    {
        fireballRainStartAudio.Play();
        StartCoroutine(startFireBallRain());
        fireballRainMiddleAudio.Play();
    }


    private IEnumerator startFireBallRain()
    {
        float timer = 0f;
        float transitionToStartRain = 3f;
        
        float originalScale = 1f;
        float maxScale = 4f;
        
        float originalOffsetY = 0f;
        float maxOffsetY = 0.7f;

        while (timer < transitionToStartRain)
        {
            float percentageDone = timer / transitionToStartRain;
            float newScale = Mathf.Lerp(originalScale, maxScale, percentageDone);
            float newOffsetY = Mathf.Lerp(originalOffsetY, maxOffsetY, percentageDone);

            transform.localScale = new Vector3(newScale/1.1f, newScale, 1f);
            transform.localPosition = new Vector2(0f, newOffsetY);
            timer += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(fireBallRain());
    }

    private IEnumerator fireBallRain()
    {
        int numberOfFireballs = (int)Random.Range(50f, 75f);

        for (int i = 0; i < numberOfFireballs; i++)
        {
            StartCoroutine(shootFireBall());
            yield return new WaitForSeconds(Random.Range(0.15f, 0.3f));
        }

        StartCoroutine(endFireBallRain());
    }

    private IEnumerator shootFireBall()
    {
        GameObject fireBallPrefabInstance = Instantiate(fireBallPrefab, transform.position, Quaternion.identity);
        shootFireballAudio.Play();

        float timer = 0f;
        float fireBallRainDuration = 1.15f;
        Vector2 startPosition = transform.position;
        Vector2 endPosition = getRandomEndPosition();
        float jumpHeight = Random.Range(2f, 9f);

        while (timer < fireBallRainDuration && fireBallPrefabInstance != null)
        {
            float percentageElapsed = timer / fireBallRainDuration;
            float yOffset = Mathf.Sin(Mathf.PI * percentageElapsed) * jumpHeight;
            Vector2 parabolicPosition = Vector2.Lerp(startPosition, endPosition, percentageElapsed) + Vector2.up * yOffset;
            fireBallPrefabInstance.transform.position = parabolicPosition;

            timer += Time.deltaTime;

            // Check for collision with the player and destroy the fireball
            // 'parabolicPosition' is the same as 'fireBallPrefabInstance.transform.position'
            Collider2D collider = Physics2D.OverlapCircle(parabolicPosition, fireBallPrefabInstance.GetComponent<CircleCollider2D>().radius);

            if (collider != null && collider.gameObject.CompareTag("Player"))
            {
                Debug.Log("IM HERE");
                Destroy(fireBallPrefabInstance);
            }

            yield return null; // Let the physics update. Just goes to next frame to render the boss' position incrementally.
        }

        Destroy(fireBallPrefabInstance);
    }

    private Vector2 getRandomEndPosition()
    {
        // Get the camera's frustum size in world units
        Camera mainCamera = Camera.main;
        float frustumHeight = 2.0f * mainCamera.orthographicSize; // mainCamera.orthographicSize gives HALF of the vertical size of the camera view. Multiplying by 2 gives the full height of the camera view.
        float frustumWidth = frustumHeight * mainCamera.aspect; // mainCamera.aspect is (width / height). Since we already have the frustum height from previous line, we can do -> (width / height) * frustumHeight, which cancels the heights and gives us the width of the camera view.

        float mainCameraPositionX = mainCamera.transform.position.x;
        float mainCameraPositionY = mainCamera.transform.position.y;

        // Calculate the position within the width of the camera's frustum and at the top of the camera frustum
        return new Vector2(
            Random.Range((-frustumWidth / 2f) + mainCameraPositionX, (frustumWidth / 2f) + mainCameraPositionX),
            Random.Range((-frustumHeight / 2f) + mainCameraPositionY, (frustumHeight / 2f) + mainCameraPositionY)
            );
    }
    

    private IEnumerator endFireBallRain()
    {
        float timer = 0f;
        float transitionToEndRain = 3f;

        float maxScale = 4f;
        float originalScale = 1f;

        float maxOffsetY = 0.7f;
        float originalOffsetY = 0f;

        while (timer < transitionToEndRain)
        {
            float percentageDone = timer / transitionToEndRain;
            float newScale = Mathf.Lerp(maxScale, originalScale, percentageDone);
            float newOffsetY = Mathf.Lerp(maxOffsetY, originalOffsetY, percentageDone);

            transform.localScale = new Vector3(newScale / 1.1f, newScale, 1f);
            transform.localPosition = new Vector2(0f, newOffsetY);
            timer += Time.deltaTime;
            yield return null;
        }

        fireballRainMiddleAudio.Stop();
        fireballRainEndAudio.Play();
        Destroy(gameObject);
    }
}