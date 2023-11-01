using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBullet : MonoBehaviour
{
    [SerializeField] GameObject explosion;
    [SerializeField] AnimationClip explosionLength;
    [SerializeField] GameObject greenBallPrefab;

    private float greenBallsSpeed = 9f;

    private int numberOfGBalls = 10;
    private float angleIncrement;

    // Start is called before the first frame update
    void Start()
    {
        angleIncrement = 360f / numberOfGBalls;

        StartCoroutine(triggerExplosion());
    }

    IEnumerator triggerExplosion()
    {
        // Let the main bullet travel for 0.9 seconds before exploding
        yield return new WaitForSeconds(0.9f);

        GameObject explosionAnim = Instantiate(explosion, transform.position, Quaternion.identity);

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

            Destroy(gBall, Random.Range(0.3f, 1.3f));
        }
    }

}