using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesDead : MonoBehaviour
{
    public List<Enemy> enemies = new List<Enemy>();
    public bool NextLevel = false;

    void Start()
    {
        Enemy[] obj = gameObject.GetComponentsInChildren<Enemy>();
        foreach(Enemy enemy in obj) 
        { 
            enemies.Add(enemy);
        }
    }

    void FixedUpdate()
    {
        if(enemies.Count == 0) 
        {
            NextLevel = true;
        }
    }
}
