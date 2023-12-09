using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireball : MonoBehaviour
{
    // THIS SCRIPT IS ONLY ATTACHED TO fireCircle PREFAB (the fireballs from the fire circle)
    // I tried attaching this to the fireBall prefab (from the rain), but it wasn't detecting collision for whatever reason
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player>().TakeDamage(1f);
            Destroy(gameObject);
        }
    }
}
