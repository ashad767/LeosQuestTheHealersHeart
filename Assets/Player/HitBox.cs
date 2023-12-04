using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public List<GameObject> hitEnimies;
    public string tag;

    private void OnEnable()
    {
        hitEnimies = new List<GameObject>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!hitEnimies.Contains(collision.gameObject) && collision.gameObject.CompareTag(tag)) 
            hitEnimies.Add(collision.gameObject);
    }

    private void OnDisable()
    {
        Debug.Log(hitEnimies.Count);
    }
}
