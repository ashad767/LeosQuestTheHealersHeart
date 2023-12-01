using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArrow : MonoBehaviour
{
    public float speed;
    public float damage;
    protected Rigidbody2D rb;
    protected BoxCollider2D boxCollider2D;
    protected float distance = 10;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    protected virtual void Update()
    {
        Despawn();
        if(rb != null)
            rb.velocity = transform.right * speed;
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

        if (collision.CompareTag("Enemy"))
        {
            rb.velocity = Vector2.zero;
            Destroy(rb);
            transform.parent = collision.transform;
            distance = 4;
            Destroy(boxCollider2D);
            if (hitObject != null)
            {
                hitObject.TakeDamage(damage);

            }

        }
        
    }
}
