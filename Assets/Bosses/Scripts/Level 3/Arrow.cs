using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody2D rb;

    public Vector2 arrowDir;
    private float arrowSpeed = 13.5f;

    private float rotation_in_Degrees;

    [SerializeField] private darknessManager darknessManager;
    [SerializeField] private GameObject firePointLight;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        rotation_in_Degrees = Mathf.Atan2(arrowDir.y, arrowDir.x) * Mathf.Rad2Deg; // Atan2 goes from -pi to pi (counter-clockwise starting from the left side). And since Atan2 returns in radians, I multiplied by Mathf.Rad2Deg to convert to degrees
        transform.rotation = Quaternion.Euler(0, 0, rotation_in_Degrees); // apply rotation on z axis

        firePointLight.GetComponent<Light>().intensity = 0.5f; // for the fire to have a lighting effect in the dark

        rb.velocity = arrowDir.normalized * arrowSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        // If the arrow is exiting the camera's view, destroy it
        if (!IsWithinCameraFrustum())
        {
            Destroy(gameObject);
        }
    }

    //Check if the arrow is within the camera's view
    bool IsWithinCameraFrustum()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        BoxCollider2D arrowCollider = GetComponent<BoxCollider2D>();

        return GeometryUtility.TestPlanesAABB(planes, arrowCollider.bounds);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player>().TakeDamage(2f);
            rb.velocity = Vector2.zero;
            transform.SetParent(collision.transform);
            Destroy(gameObject, 2f);
        }
    }
}
