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
    public Vector3 direction;
    public Transform PlayerTransform;
    public GameObject FireEffect;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        direction = (PlayerTransform.position - transform.position);
        //direction.y += 0.5f;

        float rot = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;

        transform.Rotate(0, 0, rot + 90);

        if (rb != null)
            rb.velocity = direction.normalized * speed;
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

        if (collision.CompareTag("Player"))
        {
            Ability ab = GetComponentInParent<Ability>();
            
            if(ab != null)
            {
                if(!ab.collided)
                {
                    ab.collided = true;
                    //ab.OnAbility();
                }
                if (ab is SR_Ability && ab.chance == 0)
                {
                    GameObject addedEffect = Instantiate(FireEffect);
                    addedEffect.transform.SetParent(collision.transform);
                }
            }
        }

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

    public void CheckStun(Player player)
    {

    }
}
