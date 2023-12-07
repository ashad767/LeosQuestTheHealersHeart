using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Entity : MonoBehaviour
{
    [Header("Entity")]
    public float maxHealth;
    protected float CurrentHealth;
    public bool IsRooted = false;
    public float RootTimer = 2f;
    private Rigidbody2D RB;

    protected virtual void Start()
    {
        CurrentHealth = maxHealth;
        RB = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {

        if (IsRooted)
            Rooted();
    }

    public virtual float GetHealth()
    {
        return CurrentHealth;
    }

    public virtual void Rooted()
    {
        RootTimer -= Time.deltaTime;

        if (RootTimer <= 0)
        {
            IsRooted = false;
            RootTimer = 2f;
            RB.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    public virtual void TakeDamage(float damage)
    {
        CurrentHealth -= damage;

        if (CurrentHealth < 0)
        {
            CurrentHealth = 0;
        }

       Debug.Log(name + "'s health is now " + CurrentHealth);
    }
}
