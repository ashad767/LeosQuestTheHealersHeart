using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBullet : MonoBehaviour
{
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] AnimationClip explosionLength;
    [SerializeField] GameObject greenBallPrefab;
    public GameObject audioManager;

    private float greenBallsSpeed = 9f;

    private int numberOfGBalls = 10;
    private float angleIncrement;

    private float timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        angleIncrement = 360f / numberOfGBalls;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        // If the main bullet is exiting the camera's view, trigger explosion
        if (!IsWithinCameraFrustum())
        {
            triggerExplosion();
        }

        // Else, check if the main bullet travelled for 0.9 or more seconds before exploding
        else if(timer >= 1f)
        {
            triggerExplosion();

        }
    }

    //Check if the main bullet is within the camera's view
    bool IsWithinCameraFrustum()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        BoxCollider2D bulletCollider = GetComponent<BoxCollider2D>();

        return GeometryUtility.TestPlanesAABB(planes, bulletCollider.bounds);
    }

    void triggerExplosion()
    {
        GameObject explosionAnim = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        audioManager.GetComponent<AudioManager>().PlayBulletExplosionSound();

        // Instantiate/Load the green balls that shoot out after the main bullet explodes 
        greenBallsLoader();

        // Destroy() function works asynchronously
        Destroy(explosionAnim, explosionLength.length);
        Destroy(gameObject);
    }

    void greenBallsLoader()
    {
        // Green balls loader, in order from (0, 36, 72, 108, 144, 180, 216, 252, 288, 324, 360) degrees
        for (int i = 0; i < numberOfGBalls; i++)
        {
            float gBallAngle = i * angleIncrement; // to calculate the current green ball's angle that needs to travel towards
            Vector2 direction = new Vector2(Mathf.Cos(Mathf.Deg2Rad * gBallAngle), Mathf.Sin(Mathf.Deg2Rad * gBallAngle)); // You can certainly work with degrees when calculating the direction using cos and sin functions, but Unity's Mathf.Cos and Mathf.Sin functions expect the angle to be in radians, not degrees. Degrees give bizarre results here.

            GameObject gBall = Instantiate(greenBallPrefab, transform.position, Quaternion.identity);
            gBall.GetComponent<Rigidbody2D>().velocity = direction * greenBallsSpeed;

            Destroy(gBall, Random.Range(0.3f, 1.3f)); // destroy the green ball after anytime between 0.3 to 1.3 seconds
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            triggerExplosion();
        }
    }
}