using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealField : MonoBehaviour
{
    public float time;
    public float damage;

    [Space]

    private float damageTimer;
    private float timer;
    public List<Entity> hits;

    protected virtual void Update()
    {
        timer += Time.deltaTime;
        damageTimer += Time.deltaTime;

        if(damageTimer > 1)
        {
            foreach(Entity ent in hits)
            {
                if(ent != null)
                {
                    ent.TakeDamage(damage);
                }
            }

            damageTimer = 0;
        }

        if(timer > time)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hits.Contains(collision.gameObject.GetComponent<Entity>()) && collision.CompareTag("Enemy"))
            hits.Add(collision.gameObject.GetComponent<Entity>());
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {   
        hits.Remove(collision.gameObject.GetComponent<Entity>());
    }

    
    
}
