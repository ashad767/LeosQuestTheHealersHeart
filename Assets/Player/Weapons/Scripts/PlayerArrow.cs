using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArrow : MonoBehaviour
{
    public float speed;
    public float damage;
    private Rigidbody2D rb;
    private float distance = 10;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        Despawn();
        rb.velocity = transform.right * speed;
    }

    private void Despawn()
    {
        distance -= Time.deltaTime;
        if (distance < 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Entity hitObject = collision.GetComponentInParent<Entity>();


        if(hitObject != null && !hitObject.CompareTag("Player"))
        {
            hitObject.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
