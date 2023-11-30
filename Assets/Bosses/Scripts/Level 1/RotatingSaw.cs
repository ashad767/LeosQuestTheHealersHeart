using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingSaw : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] AudioSource rotatingSaws;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rotatingSaws.Play();
        rb.velocity = Vector2.left * 9.5f;
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
    }
}