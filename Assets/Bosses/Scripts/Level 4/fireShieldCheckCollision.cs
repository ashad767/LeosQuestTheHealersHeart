using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireShieldCheckCollision : MonoBehaviour
{
    public float damage = 5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player>().TakeDamage(damage);
        }
    }
}
