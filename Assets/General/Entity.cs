using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Entity : MonoBehaviour
{
    [Header("Entity")]
    public float maxHealth;
    protected float CurrentHealth;

    protected virtual void Start()
    {
        CurrentHealth = maxHealth;    
    }

    protected virtual void Update()
    {
        Debug.Log(gameObject.name + "'s health: " + CurrentHealth);
    }

    public virtual float GetHealth()
    {
        return CurrentHealth;
    }

    public virtual void TakeDamage(float damage)
    {
        CurrentHealth -= damage;

        if (CurrentHealth < 0)
        {
            CurrentHealth = 0;
        }
    }
}
