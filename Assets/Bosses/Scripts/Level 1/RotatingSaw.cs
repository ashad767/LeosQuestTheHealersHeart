using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingSaw : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] AudioSource rotatingSaws;

    private bool playerHit = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rotatingSaws.Play();
        rb.velocity = Vector2.left * 20f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, 1260 * Time.deltaTime); // rotates 3.5x per second (360 * 3.5 = 1260)

        // Main camera's viewport goes from (0,0) (bottom left of screen) to (1,1) (top right of screen)
        // In this case, WorldToViewportPoint() transforms the saw's game world position to the camera's viewport position
        Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position);
        if (screenPos.x < 0f)
        {
            Destroy(gameObject);
        }

        // because I don't want to push the player off screen, I set the saw to isTrigger before it exits so it can pass through the player
        if(screenPos.x <= 0.25f)
        {
            GetComponent<CircleCollider2D>().isTrigger = true;
        }
    }

    #region Continuous damage logic
    // Same steps as 'OnTriggerEnter2D()'
    private void OnTriggerStay2D(Collider2D collision)
    {
        OnTriggerEnter2D(collision);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!playerHit)
            {
                collision.gameObject.GetComponent<Player>().TakeDamage(1f);
                playerHit = true; // Used as a flag in case of repeated inflicted damage on player
                StartCoroutine(waitForNextDamageTick());
            }
        }
    }

    private IEnumerator waitForNextDamageTick()
    {
        yield return new WaitForSeconds(0.3f);
        playerHit = false; // Reset the attack flag to let the next attack audio & animation play (if any)
    }
    #endregion
}