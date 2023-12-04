using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectParticles : MonoBehaviour
{
    public string effectName;
    public float effectDuration;
    public float effectDamage;
    

    private SpriteRenderer spriteRenderer;
    private SpriteRenderer parentSpriteRenderer;
    private Entity entity;
    private float damageTimer;

    private void Start()
    {
        entity = GetComponentInParent<Entity>();
        parentSpriteRenderer = transform.parent.GetComponentInChildren<SpriteRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();    
    }

    private void Update()
    {
        effectDuration -= Time.deltaTime;
        spriteRenderer.sprite = parentSpriteRenderer.sprite;
        transform.position = transform.parent.position;

        damageTimer += Time.deltaTime;

        if(entity != null && damageTimer >= 1)
        {
            entity.TakeDamage(effectDamage);
            damageTimer = 0;
        }

        if (effectDuration <= 0)
        {
            entity.TakeDamage(effectDamage);
            Destroy(gameObject);
        }
    }
}
