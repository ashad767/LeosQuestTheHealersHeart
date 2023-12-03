using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed;
    public float damage;
    protected Rigidbody2D rb;
    protected BoxCollider2D boxCollider2D;
    protected float distance = 10;
    protected Vector2 arrowVelocity;
    public Vector2 direction;
    public Transform PlayerTransform;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        direction = (PlayerTransform.position - transform.position).normalized;

        if (rb != null)
            rb.velocity = direction * speed;
    }

    protected virtual void Update()
    {
        Despawn();
    }

    protected virtual void Despawn()
    {
        distance -= Time.deltaTime;
        if (distance < 0)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        Entity hitObject = collision.GetComponentInParent<Entity>();

        if (collision.CompareTag("Player") || collision.CompareTag("WALL"))
        {
            //arrowVelocity = rb.velocity;
            rb.velocity = Vector2.zero;
            Destroy(rb);
            transform.parent = collision.transform;
            distance = 0;
            Destroy(boxCollider2D);
            if (hitObject != null)
            {
                hitObject.TakeDamage(damage);
            }
            //Destroy(gameObject);
        }
    }
}
