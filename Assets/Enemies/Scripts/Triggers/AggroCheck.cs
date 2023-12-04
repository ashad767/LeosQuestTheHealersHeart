using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggroCheck : MonoBehaviour
{
    public GameObject Player;
    public Enemy enemy;

    // Start is called before the first frame update
    private void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player");  
        enemy = GetComponentInParent<Enemy>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject == Player) 
        {
            enemy.SetAggroStatus(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == Player)
        {
            enemy.SetAggroStatus(false);
        }
    }
}
